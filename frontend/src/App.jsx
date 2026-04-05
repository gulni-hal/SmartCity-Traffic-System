import { useState } from 'react'

function App() {
    const [token, setToken] = useState('')
    const [status, setStatus] = useState('')

    // Tablo State'leri
    const [activeTab, setActiveTab] = useState('fines') // 'fines' veya 'logs'
    const [tableData, setTableData] = useState([])

    // 1. AUTH SERVİSİ
    const handleLogin = async () => {
        try {
            const credentials = { username: "admin", password: "supergizlisifre" };
            await fetch('http://localhost:5000/api/auth/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(credentials)
            });

            const res = await fetch('http://localhost:5000/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(credentials)
            });

            const data = await res.json();
            if (res.ok) {
                setToken(data.data.token);
                setStatus('Giriş başarılı! Token alındı.');
                loadTableData('fines', data.data.token); // Giriş yapınca tabloyu doldur
            } else {
                setStatus('Giriş başarısız: ' + (data.error || 'Kullanıcı bulunamadı'));
            }
        } catch (err) {
            setStatus('Sunucuya bağlanılamadı.');
        }
    }

    // 2. TRAFFIC SERVİSİ
    const sendTrafficData = async () => {
        if (!token) return setStatus('Önce giriş yapmalısınız!')
        try {
            const res = await fetch('http://localhost:5000/api/traffic/live', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
                body: JSON.stringify({ locationId: "Kadikoy", vehicleCount: Math.floor(Math.random() * 100), densityLevel: "High" })
            });
            if (res.ok) {
                setStatus('Trafik verisi (Traffic API) başarıyla gönderildi!');
                if (activeTab === 'logs') loadTableData('logs', token); // Log sekmesindeyse tabloyu güncelle
            }
        } catch (err) { setStatus('Hata oluştu.'); }
    }

    const getHotspots = async () => {
        if (!token) return setStatus('Önce giriş yapmalısınız!')
        try {
            const res = await fetch('http://localhost:5000/api/traffic/hotspots', {
                method: 'GET', headers: { 'Authorization': `Bearer ${token}` }
            });
            if (res.ok) {
                setStatus('Hotspot sorgusu (Traffic API) başarıyla çalıştı!');
                if (activeTab === 'logs') loadTableData('logs', token);
            }
        } catch (err) { setStatus('Hata oluştu.'); }
    }

    // 3. FINE SERVİSİ
    const sendFineData = async () => {
        if (!token) return setStatus('Önce giriş yapmalısınız!')
        try {
            const res = await fetch('http://localhost:5000/api/fines', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
                body: JSON.stringify({ licensePlate: "34TEST" + Math.floor(Math.random() * 99), amount: 1500, reason: "Hız İhlali" })
            });
            if (res.ok) {
                setStatus('Ceza kaydı (Fine API) başarıyla oluşturuldu!');
                if (activeTab === 'fines') loadTableData('fines', token); // Ceza sekmesindeyse tabloyu güncelle
            }
        } catch (err) { setStatus('Hata oluştu.'); }
    }

    // --- YENİ: TABLO VERİSİ ÇEKME VE MOCK VERİ ÜRETME ---
    // --- YENİ: TABLO VERİSİ ÇEKME VE KISITLAMA (PERFORMANS DÜZELTMESİ) ---
    const loadTableData = async (type, currentToken = token) => {
        setActiveTab(type);
        if (!currentToken) return setStatus('Tabloları görmek için önce giriş yapmalısınız!');

        try {
            const endpoint = type === 'fines' ? '/api/fines' : '/api/auditlogs';
            const res = await fetch(`http://localhost:5000${endpoint}`, {
                headers: { 'Authorization': `Bearer ${currentToken}` }
            });

            if (res.ok) {
                const data = await res.json();
                let parsedData = Array.isArray(data) ? data : data.data || [];

                // KRİTİK NOKTA: K6 testinden kalan 60 bin log tarayıcıyı çökertmesin diye
                // veriyi tersine çevirip (en yeniler en üste) sadece son 50 kaydı alıyoruz.
                const limitedData = parsedData.reverse().slice(0, 50);

                setTableData(limitedData);
                setStatus(`${type} verisi başarıyla yüklendi (Son 50 kayıt gösteriliyor).`);
            } else {
                // Backend GET ucu henüz hazır değilse örnek veri göster
                generateMockData(type);
                setStatus(`Backend'de GET ucu bulunamadı, ${type} için örnek veri gösteriliyor.`);
            }
        } catch (err) {
            generateMockData(type);
            setStatus(`Bağlantı hatası, ${type} için örnek veri gösteriliyor.`);
        }
    }

    const generateMockData = (type) => {
        const now = new Date().toLocaleTimeString();
        if (type === 'fines') {
            setTableData([
                { id: 1, plate: `34ABC${Math.floor(Math.random() * 99)}`, amount: '1500 TL', reason: 'Hız İhlali', date: now },
                { id: 2, plate: `06XYZ${Math.floor(Math.random() * 99)}`, amount: '800 TL', reason: 'Kırmızı Işık', date: now },
                { id: 3, plate: `35QWE${Math.floor(Math.random() * 99)}`, amount: '450 TL', reason: 'Hatalı Park', date: now }
            ]);
        } else {
            setTableData([
                { id: 1, method: 'POST', path: '/api/fines', code: 201, service: 'fine-service', date: now },
                { id: 2, method: 'POST', path: '/api/traffic/live', code: 201, service: 'traffic-service', date: now },
                { id: 3, method: 'GET', path: '/api/traffic/hotspots', code: 200, service: 'traffic-service', date: now }
            ]);
        }
    }

    return (
        <div style={{ display: 'flex', flexDirection: 'column', height: '100vh', padding: '30px', fontFamily: 'sans-serif', boxSizing: 'border-box', backgroundColor: '#f0f2f5' }}>

            <h1 style={{ textAlign: 'center', marginBottom: '20px', color: '#1a1a1a' }}>Akıllı Şehir Trafik Kontrol Paneli</h1>

            {/* 1. ÜST SATIR: KONTROL PANELİ */}
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: '30px', marginBottom: '20px', padding: '25px', background: 'white', borderRadius: '10px', boxShadow: '0 4px 6px rgba(0,0,0,0.05)', alignItems: 'flex-start' }}>
                <div style={{ borderRight: '2px solid #eee', paddingRight: '30px', flex: '1 1 auto' }}>
                    <h3 style={{ marginTop: 0, marginBottom: '15px', color: '#444' }}>Yetkilendirme</h3>
                    <button onClick={handleLogin} style={{ padding: '10px 15px', cursor: 'pointer', background: '#28a745', color: 'white', border: 'none', borderRadius: '5px', fontWeight: 'bold', width: '100%' }}>Giriş Yap & Token Al</button>
                </div>

                <div style={{ borderRight: '2px solid #eee', paddingRight: '30px', flex: '1 1 auto' }}>
                    <h3 style={{ marginTop: 0, marginBottom: '15px', color: '#444' }}>Trafik Servisi</h3>
                    <div style={{ display: 'flex', gap: '10px' }}>
                        <button onClick={sendTrafficData} style={{ padding: '10px', cursor: 'pointer', background: '#007bff', color: 'white', border: 'none', borderRadius: '5px', fontWeight: 'bold', flex: 1 }}>Trafik Bildir</button>
                        <button onClick={getHotspots} style={{ padding: '10px', cursor: 'pointer', background: '#17a2b8', color: 'white', border: 'none', borderRadius: '5px', fontWeight: 'bold', flex: 1 }}>Hotspot Sorgula</button>
                    </div>
                </div>

                <div style={{ flex: '1 1 auto' }}>
                    <h3 style={{ marginTop: 0, marginBottom: '15px', color: '#444' }}>Ceza Servisi</h3>
                    <button onClick={sendFineData} style={{ padding: '10px 15px', cursor: 'pointer', background: '#dc3545', color: 'white', border: 'none', borderRadius: '5px', fontWeight: 'bold', width: '100%' }}>Ceza Yaz</button>
                </div>

                <div style={{ width: '100%', marginTop: '5px', paddingTop: '15px', borderTop: '1px dashed #ddd', fontSize: '15px' }}>
                    <strong>Durum: </strong> <span style={{ color: '#d35400', fontWeight: 'bold' }}>{status}</span>
                </div>
            </div>

            {/* 2. ALT SATIR: İKİYE BÖLÜNMÜŞ EKRAN (TABLOLAR VE GRAFANA) */}
            <div style={{ display: 'flex', gap: '20px', flex: 1, minHeight: '0' }}>

                {/* SOL TARAF: VERİ TABLOLARI */}
                <div style={{ flex: '0 0 40%', background: 'white', borderRadius: '10px', boxShadow: '0 4px 6px rgba(0,0,0,0.05)', display: 'flex', flexDirection: 'column', overflow: 'hidden' }}>

                    {/* Tablo Sekmeleri */}
                    <div style={{ display: 'flex', background: '#f8f9fa', borderBottom: '2px solid #dee2e6' }}>
                        <button onClick={() => loadTableData('fines')} style={{ flex: 1, padding: '15px', cursor: 'pointer', border: 'none', background: activeTab === 'fines' ? 'white' : 'transparent', fontWeight: activeTab === 'fines' ? 'bold' : 'normal', borderBottom: activeTab === 'fines' ? '3px solid #dc3545' : 'none' }}>
                            Son Kesilen Cezalar
                        </button>
                        <button onClick={() => loadTableData('logs')} style={{ flex: 1, padding: '15px', cursor: 'pointer', border: 'none', background: activeTab === 'logs' ? 'white' : 'transparent', fontWeight: activeTab === 'logs' ? 'bold' : 'normal', borderBottom: activeTab === 'logs' ? '3px solid #6c757d' : 'none' }}>
                            Sistem Logları (Audit)
                        </button>
                    </div>

                    {/* Tablo İçeriği (Kaydırılabilir) */}
                    <div style={{ flex: 1, overflowY: 'auto', padding: '10px' }}>
                        <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
                            <thead>
                                <tr style={{ background: '#f1f3f5', fontSize: '14px', color: '#495057' }}>
                                    {activeTab === 'fines' ? (
                                        <>
                                            <th style={{ padding: '12px 8px', borderBottom: '2px solid #dee2e6' }}>Plaka</th>
                                            <th style={{ padding: '12px 8px', borderBottom: '2px solid #dee2e6' }}>Tutar</th>
                                            <th style={{ padding: '12px 8px', borderBottom: '2px solid #dee2e6' }}>Neden</th>
                                            <th style={{ padding: '12px 8px', borderBottom: '2px solid #dee2e6' }}>Zaman</th>
                                        </>
                                    ) : (
                                        <>
                                            <th style={{ padding: '12px 8px', borderBottom: '2px solid #dee2e6' }}>Metod/Path</th>
                                            <th style={{ padding: '12px 8px', borderBottom: '2px solid #dee2e6' }}>Servis</th>
                                            <th style={{ padding: '12px 8px', borderBottom: '2px solid #dee2e6' }}>Kod</th>
                                            <th style={{ padding: '12px 8px', borderBottom: '2px solid #dee2e6' }}>Zaman</th>
                                        </>
                                    )}
                                </tr>
                            </thead>
                            <tbody>
                                {tableData.length === 0 ? (
                                    <tr><td colSpan="4" style={{ padding: '20px', textAlign: 'center', color: '#6c757d' }}>Veri bulunamadı.</td></tr>
                                ) : tableData.map((row, index) => (
                                    <tr key={index} style={{ borderBottom: '1px solid #eee', fontSize: '14px' }}>
                                        {activeTab === 'fines' ? (
                                            <>
                                                <td style={{ padding: '12px 8px', fontWeight: 'bold' }}>{row.plate || row.licensePlate}</td>
                                                <td style={{ padding: '12px 8px', color: '#dc3545', fontWeight: 'bold' }}>{row.amount}</td>
                                                <td style={{ padding: '12px 8px' }}>{row.reason}</td>
                                                <td style={{ padding: '12px 8px', color: '#6c757d' }}>{row.date || row.createdAt}</td>
                                            </>
                                        ) : (
                                            <>
                                                <td style={{ padding: '12px 8px' }}><span style={{ background: row.method === 'GET' ? '#17a2b8' : '#28a745', color: 'white', padding: '2px 6px', borderRadius: '4px', fontSize: '12px', marginRight: '5px' }}>{row.method}</span> {row.path}</td>
                                                <td style={{ padding: '12px 8px' }}>{row.service || row.targetService}</td>
                                                <td style={{ padding: '12px 8px' }}><span style={{ color: row.code === 201 || row.code === 200 || row.statusCode === 201 ? 'green' : 'red', fontWeight: 'bold' }}>{row.code || row.statusCode}</span></td>
                                                <td style={{ padding: '12px 8px', color: '#6c757d' }}>{row.date || row.createdAt}</td>
                                            </>
                                        )}
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>

                {/* SAĞ TARAF: GRAFANA PANELİ (IFRAME) */}
                <div style={{ flex: '0 0 58%', border: 'none', borderRadius: '10px', overflow: 'hidden', boxShadow: '0 4px 6px rgba(0,0,0,0.05)', background: 'white' }}>
                    <iframe

                        src="http://localhost:3000/d/adsd6gh/system-overview?orgId=1&from=now-5m&to=now&timezone=browser"
                        width="100%"
                        height="100%"
                        frameBorder="0"
                        title="Grafana Dashboard"
                    ></iframe>
                </div>

            </div>
        </div>
    )
}

export default App