using System;
using System.IO;
using System.Linq;
using iText;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;

namespace App.PdfUtils
{
    /// <summary>
    /// Classe per eseguire il merge di PDF
    /// </summary>
    public class PdfMerge
    {
        private Stream mStream;
        private PdfDocument mDocument;
        private PdfMerger mMerge;

        /// <summary>
        /// Identificativo (Nome file o altro)
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Numero pagine incluse
        /// </summary>
        public int NumPagesMerged { get; private set; }

        /// <summary>
        /// Numero documenti inclusi
        /// </summary>
        public int NumDocumentsMerged { get; private set; }

        /// <summary>
        /// Indica se merge in corso
        /// </summary>
        public bool IsMerging
        {
            get
            {
                return (this.mDocument != null && !this.mDocument.IsClosed());
            }
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        static PdfMerge()
        {
            //PdfContext.Init();
        }

        /// <summary>
        /// Imposta le informazione del file merged
        /// </summary>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="subject"></param>
        public void SetPdfInfo(string title, string author, string subject)
        {
            this.checkMerge();

            var info = this.mDocument.GetDocumentInfo();
            info.SetTitle(title);
            info.SetAuthor(author);
            info.SetSubject(subject);
        }

        /// <summary>
        /// Aggiunge PDF da stream. Attenzione al Position dello stream!!
        /// E' possibile specificare e pagine da aggiungere. Se pages null verranno incluse tutte le pagine
        /// </summary>
        /// <param name="stream"></param>
        public void AddPdfFromStream(Stream stream, params int[] pages)
        {
            PdfReader reader = new PdfReader(stream);
            try
            {
                this.AddPdfFromReader(reader, pages);
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// Aggiunge un immagine come pagina al documento
        /// </summary>
        /// <param name="buffer"></param>
        public void AddPdfFromImageBuffer(byte[] buffer, params int[] pages)
        {
            var conv = new PdfConvert();

            using (var ms = new MemoryStream())
            {
                conv.ImageBufferToPdfStream(buffer, ms);
                ms.Position = 0;

                this.AddPdfFromStream(ms);
            }

        }

        /// <summary>
        /// Aggiunge PDF da buffer, e' possibile specificare e pagine da aggiungere. Se pages null verranno incluse tutte le pagine
        /// </summary>
        /// <param name="buffer"></param>
        public void AddPdfFromBuffer(byte[] buffer, params int[] pages)
        {
            using (var ms = new MemoryStream(buffer))
            {
                this.AddPdfFromStream(ms, pages);
            }

        }

        /// <summary>
        /// Aggiunge pdf da file, e' possibile specificare e pagine da aggiungere. Se pages null verranno incluse tutte le pagine
        /// </summary>
        /// <param name="filepath"></param>
        public void AddPdfFromFile(string filepath, params int[] pages)
        {
            PdfReader reader = new PdfReader(filepath);
            try
            {
                this.AddPdfFromReader(reader, pages);
            }
            finally
            {
                reader.Close();
            }
        }


        /// <summary>
        /// Aggiunge Pdf da reader specificando le pagine
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="pages"></param>
        public void AddPdfFromReader(PdfReader reader, params int[] pages)
        {
            this.AddPdfFromDocument(new PdfDocument(reader));
        }


        public void AddPdfFromDocument(PdfDocument doc, params int[] pages)
        {
            this.checkMerge();

            doc.GetReader().SetUnethicalReading(true);

            if (pages == null || pages.Length == 0)
            {
                var totPages = doc.GetNumberOfPages();

                for (int i = 1; i <= totPages; i++)
                {
                    try
                    {
                        this.mMerge.Merge(doc, i, i);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{nameof(PdfUtils.PdfMerge)} - Errore merge in pagina {i} di {totPages}: {e.Message}");
                        this.AddBlankPageWithText($"Si è verificato un errore nella lettura della pagina {i} di {totPages}. Il documento potrebbe non essere completo.");
                    }
                }

                this.NumPagesMerged += totPages;
            }
            else
            {
                this.mMerge.Merge(doc, pages.ToList());
                this.NumPagesMerged += pages.Length;
            }

            this.NumDocumentsMerged++;
        }


        /// <summary>
        /// Aggiunge una pagina con il testo specificato
        /// </summary>
        /// <param name="text"></param>
        /// <param name="margintop"></param>
        /// <param name="marginleft"></param>
        public void AddBlankPageWithText(string text, float margintop = 20f, float marginleft = 20f, bool alignCenter = false)
        {
            //Aggiunge pagina vuota per evitare eccezione
            var page = this.mDocument.AddNewPage(PageSize.A4);

            iText.Kernel.Pdf.Canvas.PdfCanvas pdfCanvas = new iText.Kernel.Pdf.Canvas.PdfCanvas(page);
            var canvas = new Canvas(pdfCanvas, page.GetCropBox());
            var p = new iText.Layout.Element.Paragraph(text);
            if (alignCenter)
                p.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
            else
                p.SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT);
            p.SetMarginTop(margintop);
            p.SetMarginLeft(marginleft);
            canvas.Add(p);
            canvas.Close();

            this.NumPagesMerged++;
            this.NumDocumentsMerged++;

        }

        /// <summary>
        /// Avvia merge
        /// </summary>
        /// <param name="name"></param>
        /// <param name="outstream"></param>
        /// <param name="closeOnEndMerge"></param>
        public void BeginMergeToStream(string name, Stream outstream, bool closeOnEndMerge)
        {
            this.NumDocumentsMerged = 0;
            this.NumPagesMerged = 0;
            this.Name = name;
            this.mStream = outstream;

            var wp = new WriterProperties().AddXmpMetadata();

            wp.SetPdfVersion(PdfVersion.PDF_1_4);

            var w = new PdfWriter(outstream, wp);
            w.SetCloseStream(closeOnEndMerge);
            w.SetSmartMode(true);

            this.mDocument = new PdfDocument(w);
            this.mMerge = new PdfMerger(this.mDocument);

        }

        /// <summary>
        /// Avvia merge
        /// </summary>
        /// <param name="filepath"></param>
        public void BeginMergeToFile(string filepath)
        {
            this.BeginMergeToStream(filepath, File.OpenWrite(filepath), true);
        }


        /// <summary>
        /// Termina merge
        /// </summary>
        public void EndMerge()
        {
            this.checkMerge();

            //Se non ci sono pagine ne crea una vuota
            if (this.NumDocumentsMerged <= 0 && this.NumPagesMerged <= 0)
            {
                //Aggiunge pagina vuota per evitare eccezione
                this.mDocument.AddNewPage(PageSize.A4);
            }

            this.mDocument.Close();
        }

        /// <summary>
        /// Verifica se merge in corso
        /// </summary>
        private void checkMerge()
        {
            if (!this.IsMerging)
                throw new ApplicationException("Non e' stato richiamato il metodo BeginMerge!");

        }

    }
}
