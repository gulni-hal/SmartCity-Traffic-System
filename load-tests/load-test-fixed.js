import http from 'k6/http';
import { check, sleep } from 'k6';
import { htmlReport } from 'https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js';
import { textSummary } from 'https://jslib.k6.io/k6-summary/0.0.1/index.js';

const vus = Number(__ENV.TARGET_VUS || 50);

export const options = {
  scenarios: {
    steady_load: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '30s', target: vus },
        { duration: '2m', target: vus },
        { duration: '20s', target: 0 },
      ],
      gracefulRampDown: '10s',
    },
  },
  thresholds: {
    http_req_duration: ['p(95)<2000'],
    http_req_failed: ['rate<0.01'],
  },
};


export default function () {
  const baseUrl = 'http://localhost:5000';

  const username = `polis_${vus}_${__VU}_${__ITER}`;
  const password = 'supergizlisifre';

  const regPayload = JSON.stringify({
    username: username,
    password: password,
  });

  const jsonHeaders = {
    headers: { 'Content-Type': 'application/json' },
  };

  http.post(`${baseUrl}/api/auth/register`, regPayload, jsonHeaders);

  const loginRes = http.post(`${baseUrl}/api/auth/login`, regPayload, jsonHeaders);

  let token = '';

  if (loginRes.status === 200) {
    const body = JSON.parse(loginRes.body);
    token = body?.data?.token || '';
  }

  check(loginRes, {
    'login başarılı': (r) => r.status === 200,
  });

  if (token) {
    const authHeaders = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    };

    const finePayload = JSON.stringify({
      licensePlate: `34ABC${__VU}`,
      amount: 1500,
      reason: 'Hiz Ihlali',
    });

    const fineRes = http.post(`${baseUrl}/api/fines`, finePayload, authHeaders);
    check(fineRes, {
      'fine create başarılı': (r) => r.status === 201 || r.status === 200,
    });

    const hotspotRes = http.get(`${baseUrl}/api/traffic/hotspots`, authHeaders);
    check(hotspotRes, {
      'hotspots başarılı': (r) => r.status === 200,
    });
  }

  sleep(1);
}

export function handleSummary(data) {
  const suffix = `${vus}vu`;
  return {
    [`reports/summary-${suffix}.html`]: htmlReport(data),
    [`reports/summary-${suffix}.json`]: JSON.stringify(data, null, 2),
    stdout: textSummary(data, { indent: ' ', enableColors: true }),
  };
}
