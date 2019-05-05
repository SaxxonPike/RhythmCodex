using System;
using System.Buffers.Text;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Streamers;

namespace RhythmCodex.Ddr.Integration
{
    [TestFixture]
    public class Ddr573AudioIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit("wip")]
        public void Test1()
        {
            // 3626_20f7_6b
            var inputKey = Convert.FromBase64String("fTdP25YMOWiGEi2ywMmTNny1biNK1IVr/5YJe5dC560aWBUjC1q0IGD/jR8/mtCtE17Zvu8BlSa5y/KEIWvzwo92iNk3o0/Qoi1y1517v/LBw5J86LUqWJg0kduWBEmfEkOtORqF965d+LJJ8r8Xbz7QRK0yQOWnAkj8tPn61IVr//NsHjQIrefK3ZAlInOdCXvklkm/GlCBJ2qQZDXbT5YJe/LhRA5QiMWmDVyyJknWpARpzIYtP3Lt96ZI3PxjEUKEIWuWYSw8WAnnc8pVJytb/rRg6aRB775QxIEebNHuSwEmBEn6seAOmlDPvX7Y");
            var inputScramble = Convert.FromBase64String("EGYtl85dFiErXhkpZEBAbRu+/KWjg7S+y39PAqnf8oTkCVAMsYaM+TICT1YNIFZIdC2f2u3nksz8scNrRjCh67LFLxgSZ2NTHvEmC31eOWBpUmVvGvDAjZrY9YPLXwaAzfrwhaqa1293Wiz5Ekt/HygiVwY2exLkyb+S7LXqeU5EMe/fko2+k+VnQxrYNAMJfBAgbV8SP0ka0Imzyv33goW1+Dn71qCFitNGZVJYLbeHynQEKV9XJn879sHLvtzsoYqRvMoxz5akrJuR5CkZVCWjjvh8MGl2ADc9SFRkKbbI5ZOCpfwQ6d7Uocv7tuw9");
            var inputCounter = 107;
            var inputArchive = GetArchiveResource($"Ddr.mp3.zip");
            var data = inputArchive
                .First(name => name.Key.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
                .Value;
            var expected = inputArchive
                .First(name => name.Key.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                .Value;
            var decrypter = Resolve<IDdr573AudioDecrypter>();
            var observed = decrypter.Decrypt(data, inputKey, inputScramble, inputCounter);
            observed.Should().Equal(expected);
        }

        [Test]
        [Explicit("wip")]
        public void Test2()
        {
            var name = "3009_0000_00";
            var inputKey = Convert.FromBase64String("QYSCCQUSCiQUSCiQUCGgQg==");
            var inputScramble = Convert.FromBase64String("QkGEggkFEgokFEgokFAhoA==");
            File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{name}.key.bin"), inputKey);
            File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{name}.scramble.bin"), inputScramble);
        }
    }
}