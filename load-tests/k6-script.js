import http from 'k6/http';
import { check, sleep } from 'k6';

import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";
import { textSummary } from "https://jslib.k6.io/k6-summary/0.0.1/index.js";




export let options = {
    stages: [
        { duration: '1m', target: 50 },  // 1 dakika içinde 50 kullanıcıya çık ve orada kal
        { duration: '1m', target: 200 }, // Sonraki 1 dakikada 200 kullanıcıya çık
        { duration: '2m', target: 500 }, // Sonraki 2 dakikada 500 kullanıcıya çık (Zirve yük)
        { duration: '1m', target: 0 },   // Sistemi 1 dakika içinde yavaşça soğut/kapat
    ],
    thresholds: {
        http_req_duration: ['p(95)<2000'], // İsteklerin %95'i 2 saniyeden kısa sürmeli (Performans hedefi)
        http_req_failed: ['rate<0.01'],    // Hata oranı %1'den az olmalı
    },
};

export default function () {
    const baseUrl = 'http://localhost:5000'; // Dispatcher portumuz

    // Çakışmayı önlemek için her sanal kullanıcıya (VU) özel isim üretiyoruz
    const username = 'polis_' + __VU + '_' + __ITER;
    const password = 'supergizlisifre';

    // --- KAYIT OLMA (REGISTER) ---
    const regPayload = JSON.stringify({ username: username, password: password });
    const jsonHeaders = { headers: { 'Content-Type': 'application/json' } };
    http.post(`${baseUrl}/api/auth/register`, regPayload, jsonHeaders);

    // --- GİRİŞ YAPMA (LOGIN) VE TOKEN ALMA ---
    const loginRes = http.post(`${baseUrl}/api/auth/login`, regPayload, jsonHeaders);
    let token = '';

    // Giriş başarılıysa token'ı yakalıyoruz
    if (loginRes.status === 200) {
        const body = JSON.parse(loginRes.body);
        token = body.data.token;
    }

    // ---  DİĞER SERVİSLERİ KULLANMA (Sadece token varsa) ---
    if (token !== '') {
        const authHeaders = {
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        };

        // Fine (Ceza) Oluşturma İsteği
        const finePayload = JSON.stringify({
            licensePlate: '34ABC' + __VU, // Farklı plakalar
            amount: 1500,
            reason: 'Hız İhlali'
        });
        http.post(`${baseUrl}/api/fines`, finePayload, authHeaders);

        // Traffic Hotspot Sorgulama İsteği (GET isteği)
        http.get(`${baseUrl}/api/traffic/hotspots`, authHeaders);
    }

    // Gerçekçi olması için her işlemden sonra 1 saniye bekle
    sleep(1);
}

export function handleSummary(data) {
    return {
        "reports/summary.html": htmlReport(data),
        stdout: textSummary(data, { indent: " ", enableColors: true }),
    };
}