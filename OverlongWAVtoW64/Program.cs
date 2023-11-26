using System;
using SWAV = SuperWAV.SuperWAV;

namespace OverlongWAVtoW64
{
    class Program
    {

        const long bufferSizeTicks = 50000; // 10 MB?
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Need .wav filename.");
            }
            SWAV swavRead = new SWAV(args[0]);
            SWAV.WavInfo info = swavRead.getWavInfo();
            UInt64 totalTicks = info.dataLength / info.bytesPerTick;
            SWAV swavWrite = new SWAV($"{args[0]}.w64",SWAV.WavFormat.WAVE64,info.sampleRate,info.channelCount,info.audioFormat,info.bitsPerSample,info.dataLength/info.bytesPerTick);

            UInt64 currentPos = 0;
            UInt64 ticksLeft = totalTicks;

            string statusString = $"Converting {swavRead.getWavFormat()} to W64";

            Console.Write($"{statusString}: 0%                ");
            while (ticksLeft > 0)
            {
                UInt64 howMany = Math.Min(ticksLeft, bufferSizeTicks);
                float[] data = swavRead.getAs32BitFloatFast(currentPos, currentPos+howMany);
                swavWrite.writeFloatArrayFast(data, currentPos);
                currentPos += howMany;
                ticksLeft -= howMany;
                string percent = (100.0f*(float)currentPos / (float)totalTicks).ToString("#.##");
                Console.Write($"\r{statusString}: {percent}%      ");
            }
        }
    }
}
