## 4.4. Performans ve Yük Testi (Load Testing) Raporu

**Test Aracı:** k6  
**Maksimum Eşzamanlı Kullanıcı (VU):** 500  
**Test Süresi:** ~5 Dakika  

---

### Test Senaryosu

Sistemin yoğun istek altındaki davranışını ve API Gateway (Dispatcher) yönlendirme performansını ölçmek için kademeli olarak artan bir yük testi (ramp-up) uygulanmıştır. Test, sıfırdan başlayarak zirve noktası olan 500 eşzamanlı sanal kullanıcıya ulaşmış ve toplamda 5 dakika sürmüştür.

---

### Her Sanal Kullanıcı (VU) İçin Gerçekleştirilen İşlemler

Uçtan uca (E2E) test akışı kapsamında her bir sanal kullanıcı döngüsel olarak aşağıdaki adımları simüle etmiştir:

1. **Kayıt Olma (Register):**  
   Yeni bir kullanıcı adı ve şifre ile Auth servisine kayıt işlemi (BCrypt hashing tetiklenir)

2. **Giriş Yapma (Login):**  
   Kayıt olunan bilgilerle sisteme giriş yapılıp JWT doğrulama token'ının alınması

3. **Fine Oluşturma:**  
   Alınan token kullanılarak Fine (Ceza) servisine POST isteği ile yeni bir ceza kaydının eklenmesi

4. **Traffic Hotspot Sorgusu:**  
   Token kullanılarak Traffic servisinden yoğun lokasyonların (hotspots) çekilmesi

---

### Test Sonuçları

#### Genel İstatistikler

| Metrik | Değer |
|-------|------|
| Toplam Gerçekleşen İstek | 64,360 |
| Saniyedeki İstek (RPS) | 213.82 req/s |
| Tamamlanan Iterasyon | 16,090 |
| Gelen Veri (Data Received) | 16 MB (53 kB/s) |
| Giden Veri (Data Sent) | 13 MB (44 kB/s) |

---

### Yanıt Süreleri (Response Times)

| Metrik | Değer |
|------|------|
| Ortalama (AVG) | 796.01 ms |
| Minimum (MIN) | 2.16 ms |
| Medyan (MED) | 298.78 ms |
| P90 | 2.56 s |
| P95 | 3.31 s |
| Maksimum (MAX) | 8.53 s |

Not: İsteklerin %95’i 3.31 saniye veya daha kısa sürede tamamlanmıştır.

---

### Hata Oranı (Fail Rate)

| Metrik | Değer |
|------|------|
| Başarısız İstek Oranı | %12.76 |
| Başarılı İstek Sayısı | 56,147 |
| Başarısız İstek Sayısı | 8,213 |

---

### Threshold (Eşik) Durumu
**Durum:**  
Test için belirlenen hedefler:

- P95 < 2 saniye  
- Hata oranı < %1  

500 eşzamanlı kullanıcının oluşturduğu ağır yük altında bu hedefler aşılmıştır.

---
## Analiz ve Yorum

### Güçlü Yönler

- **Yüksek İstek İşleme Kapasitesi:**  
  Sistem 5 dakika gibi kısa bir sürede 64 binden fazla isteği (%87.24 başarı oranı ile) işlemeyi başarmıştır. Saniyede 213 isteğe (RPS) ulaşılması, API Gateway (Dispatcher) ve mikroservis iletişiminin temel seviyede stabil olduğunu göstermektedir.

- **Throughput (RPS):**  
  Saniyede 213 istek işlenmesi, Dispatcher (API Gateway) ve mikroservis mimarisinin temel seviyede stabil çalıştığını göstermektedir.

- **Medyan Yanıt Süresi:**  
  Medyanın 298 ms seviyesinde kalması, sistemdeki isteklerin büyük bir çoğunluğunun aslında oldukça hızlı işlendiğini kanıtlamaktadır. Gecikmelerin geneli, yükün tepe (pik) yaptığı anlarda yığılan isteklere aittir.
---

### Zayıf Yönler ve Darboğazlar (Bottlenecks)

#### 1. Hata Oranının Artışı (%12.76)

Kullanıcı sayısı 500'e ulaştığında sistem darboğaza girmiştir. Kurgulanan senaryodaki Auth servisi üzerinde sürekli çalışan "BCrypt" şifreleme algoritması, ciddi bir CPU yükü yaratmaktadır.

---

#### 2. Auth Servisi CPU Darboğazı

- Senaryoda her istekte **register + login** işlemi yapılmaktadır.
- BCrypt hashing algoritması yüksek CPU maliyetine sahiptir.
- Bu durum Auth servisinde ciddi performans düşüşüne neden olmuştur.

---

#### 3. Zincirleme Gecikme Etkisi

- Auth servisi geciktiğinde:
  - Token üretilememektedir
  - Dispatcher (YARP) timeout yaşamaktadır
  - Fine ve Traffic servisleri çağrılamamaktadır

- Maksimum yanıt süresi: **8.53 saniye**

- Toplam hataların büyük kısmı bu zincirleme etkiden kaynaklanmaktadır.

---

## İzleme ve Metrik Görselleri (Grafana Dashboards)

Sistemin yük altındaki davranışını gösteren Grafana panelleri aşağıda sunulmuştur:

| Grafik | Sonucu |
|------|------|
| Traffic Hotspot Sorgu Sayısı | <img width="1130" height="359" alt="Screenshot 2026-04-05 003621th" src="https://github.com/user-attachments/assets/0d5dd1db-71f4-426d-be30-1cf37c269ab7" />|
| Auth Login Başarı | <img width="956" height="318" alt="image" src="https://github.com/user-attachments/assets/83f5c102-bbb5-4872-aea9-c8a45c5a3069" />|
| Fine Oluşturma | <img width="951" height="291" alt="image" src="https://github.com/user-attachments/assets/83a78b29-23a1-4c2f-b628-316e41823707" />|
| P95 Response Süresi | <img width="976" height="325" alt="image" src="https://github.com/user-attachments/assets/832af039-b756-4cc3-bc20-403d9442329a" />|
| Job Bazlı Aktif İstek | <img width="951" height="316" alt="image" src="https://github.com/user-attachments/assets/5401eb12-09c8-4290-874a-075c34993c7d" />|
| Toplam İstek Hızı | <img width="966" height="282" alt="image" src="https://github.com/user-attachments/assets/778f63ee-3c4d-4fc3-aebb-a2b81c7b0306" />|

Not: Bu metrikler Prometheus üzerinden toplanarak Grafana dashboard’ları ile görselleştirilmiştir.

