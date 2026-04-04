import { useState } from 'react'

function App() {
    const [token, setToken] = useState('')
    const [status, setStatus] = useState('')

    // 1. AUTH SERVİSİ (Kayıt ve Giriş)
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
            } else {
                setStatus('Giriş başarısız: ' + (data.error || 'Kullanıcı bulunamadı'));
            }
        } catch (err) {
            setStatus('Sunucuya bağlanılamadı.');
        }
    }

    // 2. TRAFFIC SERVİSİ (Canlı Trafik Bildirimi)
    const sendTrafficData = async () => {
        if (!token) return setStatus('Önce giriş yapmalısınız!')
        try {
            const res = await fetch('http://localhost:5000/api/traffic/live', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    locationId: "Kadikoy",
                    vehicleCount: Math.floor(Math.random() * 100),
                    densityLevel: "High"
                })
            });
            if (res.ok) setStatus('Trafik verisi (Traffic API) başarıyla gönderildi!');
        } catch (err) {
            setStatus('Hata oluştu.');
        }
    }

    // 3. TRAFFIC SERVİSİ (Hotspot Sorgulama)
    const getHotspots = async () => {
        if (!token) return setStatus('Önce giriş yapmalısınız!')
        try {
            const res = await fetch('http://localhost:5000/api/traffic/hotspots', {
                method: 'GET',
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (res.ok) setStatus('Hotspot sorgusu (Traffic API) başarıyla çalıştı!');
        } catch (err) {
            setStatus('Hata oluştu.');
        }
    }

    // 4. FINE SERVİSİ (Ceza Yazma)
    const sendFineData = async () => {
        if (!token) return setStatus('Önce giriş yapmalısınız!')
        try {
            const res = await fetch('http://localhost:5000/api/fines', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    licensePlate: "34TEST" + Math.floor(Math.random() * 99),
                    amount: 1500,
                    reason: "Hız İhlali"
                })
            });
            if (res.ok) setStatus('Ceza kaydı (Fine API) başarıyla oluşturuldu!');
        } catch (err) {
            setStatus('Hata oluştu.');
        }
    }

    return (
        <div style={{ display: 'flex', flexDirection: 'column', height: '100vh', padding: '30px', fontFamily: 'sans-serif', boxSizing: 'border-box' }}>

            {/* ANA BAŞLIK */}
            <h1 style={{ textAlign: 'center', marginBottom: '40px', color: '#333' }}>
                Akıllı Şehir Trafik Kontrol Paneli
            </h1>

            {/* KONTROL PANELİ */}
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: '30px', marginBottom: '30px', padding: '30px', background: '#f8f9fa', border: '1px solid #dee2e6', borderRadius: '8px', alignItems: 'flex-start' }}>

                {/* 1. YETKİLENDİRME */}
                <div style={{ borderRight: '2px solid #ddd', paddingRight: '30px', flex: '1 1 auto' }}>
                    <h3 style={{ marginTop: 0, marginBottom: '20px', color: '#444' }}>1. Yetkilendirme</h3>
                    <button onClick={handleLogin} style={{ padding: '10px 15px', cursor: 'pointer', background: '#28a745', color: 'white', border: 'none', borderRadius: '5px', fontWeight: 'bold', width: '100%' }}>
                        Giriş Yap & Token Al
                    </button>
                </div>

                {/* 2. TRAFİK SERVİSİ */}
                <div style={{ borderRight: '2px solid #ddd', paddingRight: '30px', flex: '1 1 auto' }}>
                    <h3 style={{ marginTop: 0, marginBottom: '20px', color: '#444' }}>2. Trafik Servisi</h3>
                    <div style={{ display: 'flex', gap: '10px' }}>
                        <button onClick={sendTrafficData} style={{ padding: '10px 15px', cursor: 'pointer', background: '#007bff', color: 'white', border: 'none', borderRadius: '5px', fontWeight: 'bold', flex: 1 }}>
                            Trafik Bildir
                        </button>
                        <button onClick={getHotspots} style={{ padding: '10px 15px', cursor: 'pointer', background: '#17a2b8', color: 'white', border: 'none', borderRadius: '5px', fontWeight: 'bold', flex: 1 }}>
                            Hotspot Sorgula
                        </button>
                    </div>
                </div>

                {/* 3. CEZA SERVİSİ */}
                <div style={{ flex: '1 1 auto' }}>
                    <h3 style={{ marginTop: 0, marginBottom: '20px', color: '#444' }}>3. Ceza Servisi</h3>
                    <button onClick={sendFineData} style={{ padding: '10px 15px', cursor: 'pointer', background: '#dc3545', color: 'white', border: 'none', borderRadius: '5px', fontWeight: 'bold', width: '100%' }}>
                        Ceza Yaz
                    </button>
                </div>

                {/* DURUM BİLDİRİM ÇUBUĞU */}
                <div style={{ width: '100%', marginTop: '10px', paddingTop: '20px', borderTop: '1px dashed #ccc', fontSize: '16px' }}>
                    <strong>Son İşlem Durumu: </strong> <span style={{ color: '#d35400', fontWeight: 'bold' }}>{status}</span>
                </div>
            </div>

            {/* GRAFANA PANELİ (IFRAME) */}
            <div style={{ flex: 1, border: '2px solid #ccc', borderRadius: '8px', overflow: 'hidden', minHeight: '400px' }}>
                <iframe
                    
                    src="http://localhost:3000/d/adsd6gh/system-overview?orgId=1&from=now-5m&to=now&timezone=browser"
                    width="100%"
                    height="100%"
                    frameBorder="0"
                    title="Grafana Dashboard"
                ></iframe>
            </div>
        </div>
    )
}

export default App