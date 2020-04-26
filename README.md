# Sentetik Veri Üretme
<br/>
## Proje Amacı

Bu projede sentetik olarak doğrusal ve doğrusal olmayan veri üretmek amaçlanmıştır. Program üzerinde üretilecek veri sayısı, öznitelik sayısı, verinin işlevi ve normalizasyon yöntemleri kullanıcıya bırakılmıştır. Kullanıcı istenilen sayıda öznitelik ekleyebilir bunları isimlendirebilir ve sınırlarını yani o öznitelik için hangi aralıkta üretileceğini belirleyebilir. Daha sonra doğrusal bir veri üretilecekse istenilen bir korelasyon katsayısı belirlenir.Korelasyon katsayısı 1'e yaklaştıkça çok yüksek 0'a yaklaştıkça ise veriler arasında zayıf ilişki olduğu anlamı taşımaktadır.Korelasyon katsayısı negatif ise negatif yönde ilişki, pozitif ise pozitif yönde ilişki var demektir. Veriler belirlenen bu özellikler doğrultusunda üretirdikten sonra veri üzerinde lineer regresyon analizi yapılmaktadır. Analiz sonucunda oluşan lineer regresyon modeli kullanıcıya gösterilmektedir. Veriler kullanıcı tarafından istenilen normalize yöntemi ile normalize edilmektedir.Bu normalize yöntemleri; Z-score ve Min-Max normalizasyon yöntemleridir. En sonunda ise veri seti normalize hali ile bir excell dosyasına yazılmaktadır.

Proje C# ve R dili ile birlikte oluştururmuştur. Projein genel işleyişi C# üzerinde, istenilen korelasyon katsayısında lineer veri oluşurma ve linner regresyon kısımları ise R üzerinde yazılmıştır. R üzerinde yazılan kodlar online ortamda derlendiğinden ötürü programın çalışabilmesi için internet bağlantısına ihtiyaç duyulmaktadır.

Source klasörü altında uygulamanın kaynak kodlarını program klasörünün altın da ise uygulamayı bulabilirsiniz. 
