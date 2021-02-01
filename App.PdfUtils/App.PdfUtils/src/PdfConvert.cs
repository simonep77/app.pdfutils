using System;
using System.IO;
using System.Linq;
using iText;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;

namespace App.PdfUtils
{
    /// <summary>
    /// Classe per eseguire la conversione di tipi doc -> PDF
    /// </summary>
    public class PdfConvert
    {

        public string Author { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public float ImageScalePercent { get; set; } = 0f;

        /// <summary>
        /// Converte immagine da stream in pdf su stream
        /// </summary>
        /// <param name="srcImageStream"></param>
        /// <param name="outPdfStream"></param>
        public void ImageBufferToPdfStream(byte[] image, Stream outPdfStream)
        {



            var wp = new WriterProperties().AddXmpMetadata().SetFullCompressionMode(true).SetPdfVersion(PdfVersion.PDF_1_4);
            var w = new PdfWriter(outPdfStream, wp);
            w.SetCloseStream(false);
            var doc = new PdfDocument(w);

            var info = doc.GetDocumentInfo();
            //Dati PDF
            if (!string.IsNullOrEmpty(this.Author))
                info.SetAuthor(this.Author);

            if (!string.IsNullOrEmpty(this.Title))
                info.SetTitle(this.Title);

            if (!string.IsNullOrEmpty(this.Subject))
                info.SetSubject(this.Subject);

            var hlDoc = new Document(doc);

            var img = ImageDataFactory.Create(image, true);

            if (img is TiffImageData)
            {
                var numFrames = TiffImageData.GetNumberOfPages(image);

                for (int iPage = 1; iPage <= numFrames; iPage++)
                {
                    img = ImageDataFactory.CreateTiff(image, false, iPage, false);

                    using (var ms = new MemoryStream())
                    {
                        var imgPdf = new Image(img);
                        imgPdf.SetFixedPosition(0, 0);

                        doc.AddNewPage(new PageSize(imgPdf.GetImageWidth(), imgPdf.GetImageHeight()));

                        hlDoc.Add(imgPdf);
                    }

                }
            }
            else
            {
                doc.AddNewPage(new PageSize(img.GetWidth(), img.GetHeight()));
                var imgPdf = new Image(img);
                imgPdf.SetFixedPosition(0, 0);
                hlDoc.Add(imgPdf);
            }






            doc.Close();
         

        }




    }
}
