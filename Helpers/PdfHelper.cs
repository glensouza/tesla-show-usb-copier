using Spire.Pdf;
using Spire.Pdf.Graphics;

namespace TeslaLightShow.Helpers;

public class PdfHelper : IDisposable
{
    private readonly string dateTime;
    private readonly PdfDocument doc = new("EnvelopeTemplates.pdf");
    private readonly PdfFont pdfFont = new(PdfFontFamily.Helvetica, 12f);
    private readonly PdfSolidBrush pdfBrush = new(Color.Black);
    private readonly List<string> fileNames;
    private readonly PdfPageBase page1;
    private readonly PdfGraphicsState state1;
    private readonly PdfPageBase page2;
    private readonly PdfGraphicsState state2;
    private readonly PdfPageBase page3;
    private readonly PdfGraphicsState state3;
    private readonly PdfPageBase page4;
    private readonly PdfGraphicsState state4;
    private readonly PdfPageBase page5;
    private readonly PdfGraphicsState state5;
    private readonly PdfPageBase page6;
    private readonly PdfGraphicsState state6;

    public PdfHelper(string dateTime, List<string> fileNames)
    {
        this.dateTime = dateTime;
        this.fileNames = fileNames;

        this.page1 = this.doc.Pages[0];
        this.state1 = this.page1.Canvas.Save();
        this.page1.Canvas.TranslateTransform(225, 410);

        this.page2 = this.doc.Pages[1];
        this.state2 = this.page2.Canvas.Save();
        this.page2.Canvas.TranslateTransform(225, 360);

        this.page3 = this.doc.Pages[2];
        this.state3 = this.page3.Canvas.Save();
        this.page3.Canvas.TranslateTransform(225, 360);

        this.page4 = this.doc.Pages[3];
        this.state4 = this.page4.Canvas.Save();
        this.page4.Canvas.TranslateTransform(96, 345);
        this.page4.Canvas.ScaleTransform(1f, 0.6f);

        this.page5 = this.doc.Pages[4];
        this.state5 = this.page5.Canvas.Save();
        this.page5.Canvas.TranslateTransform(100, 220);

        this.page6 = this.doc.Pages[5];
        this.state6 = this.page6.Canvas.Save();
        this.page6.Canvas.TranslateTransform(100, 220);
    }

    public string CreatePdf()
    {
        for (int i = 0; i < 42; i++)
        {
            string fileName = this.fileNames.Count > i ? this.fileNames[i] : string.Empty;
            if (fileName.Length > 20)
            {
                fileName = fileName[..20];
            }

            if (i < 10)
            {
                this.page1.Canvas.DrawString($"{i + 1}. {(this.fileNames.Count < i + 1 ? "________________" : fileName)}", this.pdfFont, this.pdfBrush, i < 9 ? 5 : 0, (i * 17));
            }

            if (i < 13)
            {
                this.page2.Canvas.DrawString($"{i + 1}. {(this.fileNames.Count < i + 1 ? "_______________" : fileName)}", this.pdfFont, this.pdfBrush, i < 9 ? 5 : 0, (i * 17));
                this.page3.Canvas.DrawString($"{i + 1}. {(this.fileNames.Count < i + 1 ? "_______________" : fileName)}", this.pdfFont, this.pdfBrush, i < 9 ? 5 : 0, (i * 17));
            }

            if (i < 33)
            {
                int div11 = i / 11;
                int mod11 = i % 11;

                this.page4.Canvas.DrawString($"{i + 1}. {(this.fileNames.Count < i + 1 ? "______________" : fileName)}", this.pdfFont, this.pdfBrush, (i < 9 ? 5 : 0) + (div11 * 137), (mod11 * 17));
            }

            int div14 = i / 14;
            int mod14 = i % 14;

            this.page5.Canvas.DrawString($"{i + 1}. {(this.fileNames.Count < i + 1 ? "______________" : fileName)}", this.pdfFont, this.pdfBrush, (i < 9 ? 5 : 0) + (div14 * 137), (mod14 * 17));
            this.page6.Canvas.DrawString($"{i + 1}. {(this.fileNames.Count < i + 1 ? "______________" : fileName)}", this.pdfFont, this.pdfBrush, (i < 9 ? 5 : 0) + (div14 * 137), (mod14 * 17));
        }

        string tempPdfFile = $"{Path.GetTempPath()}\\TeslaLightShow_{this.dateTime}.pdf";
        this.doc.SaveToFile(tempPdfFile);
        new Process { StartInfo = new ProcessStartInfo(tempPdfFile) { UseShellExecute = true } }.Start();
        return tempPdfFile;
    }

    private void ReleaseUnmanagedResources()
    {
        this.page1.Canvas.Restore(this.state1);
        this.page2.Canvas.Restore(this.state2);
        this.page3.Canvas.Restore(this.state3);
        this.page4.Canvas.Restore(this.state4);
        this.page5.Canvas.Restore(this.state5);
        this.page6.Canvas.Restore(this.state6);
        this.doc.Dispose();
    }

    public void Dispose()
    {
        this.ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~PdfHelper()
    {
        this.ReleaseUnmanagedResources();
    }
}
