using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.PdfUtils;
using System.IO;
using System;

namespace App.PdfUtils.Test
{
    [TestClass]
    public class TestConvertTiff
    {
        [TestMethod]
        public void TestTiffMP()
        {
            var pc = new App.PdfUtils.PdfConvert();
            pc.Author = @"Simone Pelaia";
            pc.Title = "Prova Tiff multipagina";
            pc.Subject = @"Tiff MP";

            using (var ms = new MemoryStream())
            {
                pc.ImageBufferToPdfStream(TestResources.Multi_page24bpp, ms);

                Assert.IsFalse(ms.Length == 0);
                File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "TestTiff.pdf"), ms.ToArray());
            }
        }

        //[TestMethod]
        //public void TestPng()
        //{
        //    var pc = new App.PdfUtils.PdfConvert();
        //    pc.Author = @"Simone Pelaia";
        //    pc.Title = "Prova PNG";
        //    pc.Subject = @"PNG";

        //    using (var ms = new MemoryStream())
        //    {
        //        pc.ImageBufferToPdfStream(TestResources.Prova, ms);

        //        Assert.IsFalse(ms.Length == 0);

        //        File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "TestPng.pdf"), ms.ToArray());
        //    }
        //}
    }
}
