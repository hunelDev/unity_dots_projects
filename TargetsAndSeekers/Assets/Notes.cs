
public class Notes
{
    //Step-1
    //Baktigimizda seeker da targeti aragimiz findNearest monobehaviour calisiyor ve surekli en yakindaki targeti ariyor.Seeker ve target instatiate edilirken random Random.insideUnitCircle ile -1 1 arasinda bir deger alacak Vectctor2 olarak
    //sonucta 1000 ne 1000 seeker yaratiyoruz ve buraradaki amacimiz brute brute-force algorithm yani kabakuvvet algortimayla arama yaparak sonucta profilerda update islemi kac ms cikmis ona bakiyoruz;
    //https://github.com/Unity-Technologies/EntityComponentSystemSamples/tree/master/Dots101/Jobs101/Assets/TargetsAndSeekers bu adresen de alabliriz tabi sonucu
    //bende FindNearest update 1000 instance icin toplmda 377ms geliyor tanesi 0.37 ms geliyor demektir
    
    //Step-2
    //FindNearest i Spawner e tasidik ve job a verilecek olan 3 native array tanimladik persist alloc ettik lenghtini Spawner dan aldik.
    //Job tanimladik ve seekerda yaptigimiz hesaplamlari biraz degistiridk  Unity.Mathematics; kullanarak math.distancesq kullandik.Ic ice loopla nearsti bulduk seekere gore ve joba verdimiz Native collectin NearestTargetPositions ile aldik.
    //index e gore draw ettik update icinde
    //2.5 fps uctu ve 0.7 ms main thread de seeker.update surdu sadece 1/100 une dustu ama farkettiysek bu seeker.update sadece thread deki degil burst
    //sonuca bakarsak profilerda burst 2ms suruyor toplam loop 7ms suruyor ve main thread de job execute ediyor burst de baska threadde calistiiyor ayni zamanda main thread bos durmuyor yani.
    //Bu arada burst LLVM compiler mis ve IL kodu Mankina native koda ceviriyor
    //not olarak diyorki Complate main threadde is bitmeyen joblar uzerine de cagirliyormus amac main threadinde boylece is yapmasini saglamak akti saktirde complate olmasini beklemek zorunda kalidrmis idle olarak
    //1kdan 10kya cekersek sayiyi seekerlarinve targetin 70kat performans duruyor cunku her seeker 10kat fazla arama yapacak normalin 100 kati fazla yuk demek matematiksel olarak
    //mesela profilerdan bakarsak bagzen main thread idle ken job workerlarinda burst calisiyor bagzen se o idle oluyor main calistiriyor burstu
    
}
