using Unity.Entities;

//Accessing entities in jobs
//Job sistemle enitity data islemlerini worker threads ile yapabliyoruz.Entity sistemi 2 interface iceriyormus entitlere ulasabilen job tanimlamak icin.
//IJobChunk; Query ile eslesen bir chunk icin Execute() methodunu cagriyor
//IJobEntity; Query ile eslen her entity icin Execute() methdounu cagriyor.
//IJobEntity kullanima daha uygun olmsada IJobChunk daha kesin kontrol sagliyormus.Genellikle performansli esitmis ayni worklerde.
//Not;IJobEntity aslinda gercek bir job type degildir.Source generation IJobChunk dan impelment ederek IJobEntity structer turetiyor.IJobEntity IJobChunk olarak scheduled edliyor.

//IJobChunk or IJobEntity da workleri bolmek icin coklu threadlere Schedule() yerine ScheduleParallel() cagriliyor.ScheduleParallel kullanildiginda query ile eslesn chunklar ayri batchlere konuyor ve threadlere dagitiliyor.
//Structual changeler bir jobun icinden yapilamiyorlarmis,main thread uzerinde yapilacakmis.Ancak joblar structual change commandlerini kaydebiliyorlarmis EntityCommandBuffer ile ve bu recordlar tekrar main threade oynatiliyormus.


//Sync points
//Bazi operasyonlar main thread uzerinde biraz veya tamamen scheduled jobu tamamlayan 'synchronization pointler' trigger ediyormus.