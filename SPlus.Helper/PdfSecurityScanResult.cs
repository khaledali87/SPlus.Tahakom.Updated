using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Action;
using iText.Forms;
using iText.Forms.Fields;

namespace SPlus.Helper
{
    public class PdfSecurityScanResult
    {
        public bool IsSafe { get; set; } = true;
        public bool ContainsJavaScript { get; set; }
        public bool ContainsEmbeddedFiles { get; set; }
        public bool ContainsExternalReferences { get; set; }
        public bool ContainsFormFields { get; set; }
        public bool ContainsLaunchActions { get; set; }
        public bool ContainsURIActions { get; set; }
        public bool ContainsSubmitFormActions { get; set; }
        public List<string> JavaScriptCode { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> DetectedThreats { get; set; } = new List<string>();
        public string Summary { get; set; } = string.Empty;
    }

    /// <summary>
    /// Main PDF Security Scanner class
    /// </summary>
    public class PdfSecurityScanner
    {
        private readonly List<string> dangerousPatterns = new List<string>
        {
            @"eval\s*\(",
            @"unescape\s*\(",
            @"escape\s*\(",
            @"String\.fromCharCode",
            @"document\.write",
            @"window\.location",
            @"ActiveXObject",
            @"WScript\.Shell",
            @"cmd\.exe",
            @"powershell",
            @"<script",
            @"javascript:",
            @"data:text/html"
        };

        /// <summary>
        /// Scan a PDF file for security threats
        /// </summary>
        public PdfSecurityScanResult ScanPdfFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"PDF file not found: {filePath}");
            }

            var result = new PdfSecurityScanResult();

            try
            {
                using (PdfReader reader = new PdfReader(filePath))
                using (PdfDocument pdfDoc = new PdfDocument(reader))
                {
                    // Check for JavaScript
                    CheckForJavaScript(pdfDoc, result);

                    // Check for embedded files
                    CheckForEmbeddedFiles(pdfDoc, result);

                    // Check for form fields
                    CheckForFormFields(pdfDoc, result);

                    // Check all pages for dangerous elements
                    CheckPages(pdfDoc, result);

                    // Check document catalog for scripts
                    CheckDocumentCatalog(pdfDoc, result);

                    // Check for external references
                    CheckForExternalReferences(pdfDoc, result);

                    // Generate summary
                    GenerateSummary(result);
                }
            }
            catch (Exception ex)
            {
                result.IsSafe = false;
                result.Warnings.Add($"Error scanning PDF: {ex.Message}");
                result.DetectedThreats.Add("Unable to fully scan PDF - file may be corrupted or malformed");
            }

            return result;
        }

        /// <summary>
        /// Check for JavaScript in the PDF
        /// </summary>
        private void CheckForJavaScript(PdfDocument pdfDoc, PdfSecurityScanResult result)
        {
            try
            {
                // Check document-level JavaScript
                PdfDictionary catalog = pdfDoc.GetCatalog().GetPdfObject();

                // Check Names dictionary for JavaScript
                PdfDictionary names = catalog.GetAsDictionary(PdfName.Names);
                if (names != null)
                {
                    PdfDictionary javascript = names.GetAsDictionary(PdfName.JavaScript);
                    if (javascript != null)
                    {
                        result.ContainsJavaScript = true;
                        result.IsSafe = false;
                        result.DetectedThreats.Add("Document contains embedded JavaScript in Names dictionary");
                        ExtractJavaScriptFromDictionary(javascript, result);
                    }
                }

                // Check for OpenAction with JavaScript
                PdfDictionary openAction = catalog.GetAsDictionary(PdfName.OpenAction);
                if (openAction != null)
                {
                    CheckActionForJavaScript(openAction, result, "Document OpenAction");
                }

                // Check AA (Additional Actions)
                PdfDictionary aa = catalog.GetAsDictionary(PdfName.AA);
                if (aa != null)
                {
                    result.ContainsJavaScript = true;
                    result.IsSafe = false;
                    result.DetectedThreats.Add("Document contains Additional Actions (AA) which may include JavaScript");
                }
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Error checking for JavaScript: {ex.Message}");
            }
        }

        /// <summary>
        /// Extract JavaScript code from dictionary
        /// </summary>
        private void ExtractJavaScriptFromDictionary(PdfDictionary jsDict, PdfSecurityScanResult result)
        {
            if (jsDict == null) return;

            foreach (PdfName key in jsDict.KeySet())
            {
                PdfObject obj = jsDict.Get(key);
                if (obj != null)
                {
                    if (obj.IsStream())
                    {
                        try
                        {
                            byte[] bytes = ((PdfStream)obj).GetBytes();
                            string jsCode = Encoding.UTF8.GetString(bytes);
                            if (!string.IsNullOrWhiteSpace(jsCode))
                            {
                                result.JavaScriptCode.Add(jsCode);
                                AnalyzeJavaScriptCode(jsCode, result);
                            }
                        }
                        catch { }
                    }
                    else if (obj.IsString())
                    {
                        string jsCode = ((PdfString)obj).GetValue();
                        if (!string.IsNullOrWhiteSpace(jsCode))
                        {
                            result.JavaScriptCode.Add(jsCode);
                            AnalyzeJavaScriptCode(jsCode, result);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Analyze JavaScript code for dangerous patterns
        /// </summary>
        private void AnalyzeJavaScriptCode(string jsCode, PdfSecurityScanResult result)
        {
            foreach (var pattern in dangerousPatterns)
            {
                if (Regex.IsMatch(jsCode, pattern, RegexOptions.IgnoreCase))
                {
                    result.DetectedThreats.Add($"Potentially dangerous JavaScript pattern detected: {pattern}");
                    result.IsSafe = false;
                }
            }
        }

        /// <summary>
        /// Check for embedded files
        /// </summary>
        private void CheckForEmbeddedFiles(PdfDocument pdfDoc, PdfSecurityScanResult result)
        {
            try
            {
                PdfDictionary catalog = pdfDoc.GetCatalog().GetPdfObject();
                PdfDictionary names = catalog.GetAsDictionary(PdfName.Names);

                if (names != null)
                {
                    PdfDictionary embeddedFiles = names.GetAsDictionary(PdfName.EmbeddedFiles);
                    if (embeddedFiles != null)
                    {
                        result.ContainsEmbeddedFiles = true;
                        result.Warnings.Add("PDF contains embedded files which could potentially be malicious");

                        // Check for executable file extensions
                        CheckEmbeddedFileTypes(embeddedFiles, result);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Error checking for embedded files: {ex.Message}");
            }
        }

        /// <summary>
        /// Check types of embedded files
        /// </summary>
        private void CheckEmbeddedFileTypes(PdfDictionary embeddedFiles, PdfSecurityScanResult result)
        {
            List<string> dangerousExtensions = new List<string>
            {
                ".exe", ".dll", ".bat", ".cmd", ".com", ".scr", ".vbs",
                ".js", ".jar", ".zip", ".rar", ".ps1", ".sh"
            };

            // Note: Full implementation would traverse the embedded files tree
            // and check each file's extension
            result.Warnings.Add("Embedded files detected - manual inspection recommended");
        }

        /// <summary>
        /// Check for form fields
        /// </summary>
        private void CheckForFormFields(PdfDocument pdfDoc, PdfSecurityScanResult result)
        {
            try
            {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
                if (form != null)
                {
                    var fields = form.GetAllFormFields();
                    if (fields != null && fields.Count > 0)
                    {
                        result.ContainsFormFields = true;
                        result.Warnings.Add($"PDF contains {fields.Count} form field(s)");

                        // Check each field for JavaScript actions
                        foreach (var field in fields)
                        {
                            CheckFormFieldForJavaScript(field.Value, result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Error checking form fields: {ex.Message}");
            }
        }

        /// <summary>
        /// Check form field for JavaScript
        /// </summary>
        private void CheckFormFieldForJavaScript(PdfFormField field, PdfSecurityScanResult result)
        {
            if (field == null) return;

            PdfDictionary fieldDict = field.GetPdfObject();
            if (fieldDict == null) return;

            // Check field's Additional Actions (AA)
            PdfDictionary aa = fieldDict.GetAsDictionary(PdfName.AA);
            if (aa != null)
            {
                result.ContainsJavaScript = true;
                result.IsSafe = false;
                result.DetectedThreats.Add($"Form field contains Additional Actions which may include JavaScript");
            }

            // Check field's actions
            PdfDictionary action = fieldDict.GetAsDictionary(PdfName.A);
            if (action != null)
            {
                CheckActionForJavaScript(action, result, "Form field action");
            }
        }

        /// <summary>
        /// Check all pages for dangerous elements
        /// </summary>
        private void CheckPages(PdfDocument pdfDoc, PdfSecurityScanResult result)
        {
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                try
                {
                    PdfPage page = pdfDoc.GetPage(i);

                    // Check page annotations
                    CheckPageAnnotations(page, result, i);

                    // Check page actions
                    CheckPageActions(page, result, i);
                }
                catch (Exception ex)
                {
                    result.Warnings.Add($"Error checking page {i}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Check page annotations for dangerous content
        /// </summary>
        private void CheckPageAnnotations(PdfPage page, PdfSecurityScanResult result, int pageNum)
        {
            var annotations = page.GetAnnotations();
            if (annotations == null) return;

            foreach (PdfAnnotation annot in annotations)
            {
                PdfDictionary annotDict = annot.GetPdfObject();

                // Check for JavaScript actions in annotation
                PdfDictionary action = annotDict.GetAsDictionary(PdfName.A);
                if (action != null)
                {
                    CheckActionForJavaScript(action, result, $"Page {pageNum} annotation");
                }

                // Check Additional Actions
                PdfDictionary aa = annotDict.GetAsDictionary(PdfName.AA);
                if (aa != null)
                {
                    result.ContainsJavaScript = true;
                    result.IsSafe = false;
                    result.DetectedThreats.Add($"Page {pageNum} contains annotation with Additional Actions");
                }

                // Check for URI actions
                if (action != null && PdfName.URI.Equals(action.Get(PdfName.S)))
                {
                    result.ContainsURIActions = true;
                    PdfString uri = action.GetAsString(PdfName.URI);
                    if (uri != null)
                    {
                        string url = uri.GetValue();
                        if (url.ToLower().Contains("javascript:"))
                        {
                            result.ContainsJavaScript = true;
                            result.IsSafe = false;
                            result.DetectedThreats.Add($"Page {pageNum} contains javascript: URI");
                        }
                        result.Warnings.Add($"Page {pageNum} contains external URI: {url}");
                    }
                }
            }
        }

        /// <summary>
        /// Check page actions
        /// </summary>
        private void CheckPageActions(PdfPage page, PdfSecurityScanResult result, int pageNum)
        {
            PdfDictionary pageDict = page.GetPdfObject();

            // Check page Additional Actions
            PdfDictionary aa = pageDict.GetAsDictionary(PdfName.AA);
            if (aa != null)
            {
                result.ContainsJavaScript = true;
                result.IsSafe = false;
                result.DetectedThreats.Add($"Page {pageNum} contains Additional Actions");
            }
        }

        /// <summary>
        /// Check action dictionary for JavaScript
        /// </summary>
        private void CheckActionForJavaScript(PdfDictionary action, PdfSecurityScanResult result, string location)
        {
            if (action == null) return;

            PdfName actionType = action.GetAsName(PdfName.S);

            if (actionType != null)
            {
                if (PdfName.JavaScript.Equals(actionType))
                {
                    result.ContainsJavaScript = true;
                    result.IsSafe = false;
                    result.DetectedThreats.Add($"{location} contains JavaScript action");

                    // Try to extract the JavaScript code
                    PdfObject js = action.Get(PdfName.JS);
                    if (js != null)
                    {
                        if (js.IsString())
                        {
                            string jsCode = ((PdfString)js).GetValue();
                            result.JavaScriptCode.Add(jsCode);
                            AnalyzeJavaScriptCode(jsCode, result);
                        }
                        else if (js.IsStream())
                        {
                            try
                            {
                                byte[] bytes = ((PdfStream)js).GetBytes();
                                string jsCode = Encoding.UTF8.GetString(bytes);
                                result.JavaScriptCode.Add(jsCode);
                                AnalyzeJavaScriptCode(jsCode, result);
                            }
                            catch { }
                        }
                    }
                }
                else if (PdfName.Launch.Equals(actionType))
                {
                    result.ContainsLaunchActions = true;
                    result.IsSafe = false;
                    result.DetectedThreats.Add($"{location} contains Launch action (can execute external programs)");
                }
                else if (PdfName.SubmitForm.Equals(actionType))
                {
                    result.ContainsSubmitFormActions = true;
                    result.Warnings.Add($"{location} contains SubmitForm action");
                }
                else if (PdfName.URI.Equals(actionType))
                {
                    result.ContainsURIActions = true;
                    PdfString uri = action.GetAsString(PdfName.URI);
                    if (uri != null && uri.GetValue().ToLower().Contains("javascript:"))
                    {
                        result.ContainsJavaScript = true;
                        result.IsSafe = false;
                        result.DetectedThreats.Add($"{location} contains javascript: URI");
                    }
                }
            }

            // Check for Next action (action chaining)
            PdfObject next = action.Get(PdfName.Next);
            if (next != null)
            {
                result.Warnings.Add($"{location} contains chained actions");
                if (next.IsDictionary())
                {
                    CheckActionForJavaScript((PdfDictionary)next, result, location + " (chained)");
                }
            }
        }

        /// <summary>
        /// Check document catalog for scripts
        /// </summary>
        private void CheckDocumentCatalog(PdfDocument pdfDoc, PdfSecurityScanResult result)
        {
            try
            {
                PdfDictionary catalog = pdfDoc.GetCatalog().GetPdfObject();

                // Check for Renditions (multimedia)
                if (catalog.ContainsKey(PdfName.Renditions))
                {
                    result.Warnings.Add("PDF contains multimedia renditions");
                }

                // Check for 3D content
                if (catalog.ContainsKey(new PdfName("3D")))
                {
                    result.Warnings.Add("PDF contains 3D content which may include scripts");
                }

                // Check for XFA forms (can contain JavaScript)
                PdfDictionary acroForm = catalog.GetAsDictionary(PdfName.AcroForm);
                if (acroForm != null && acroForm.ContainsKey(PdfName.XFA))
                {
                    result.ContainsJavaScript = true;
                    result.IsSafe = false;
                    result.DetectedThreats.Add("PDF contains XFA forms which can include JavaScript");
                }
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Error checking document catalog: {ex.Message}");
            }
        }

        /// <summary>
        /// Check for external references
        /// </summary>
        private void CheckForExternalReferences(PdfDocument pdfDoc, PdfSecurityScanResult result)
        {
            // This is handled in annotation checks, but we mark it here for clarity
            if (result.ContainsURIActions || result.ContainsSubmitFormActions)
            {
                result.ContainsExternalReferences = true;
            }
        }

        /// <summary>
        /// Generate summary of scan results
        /// </summary>
        private void GenerateSummary(PdfSecurityScanResult result)
        {
            StringBuilder summary = new StringBuilder();

            if (result.IsSafe)
            {
                summary.AppendLine("✓ PDF appears to be safe");
            }
            else
            {
                summary.AppendLine("⚠ PDF contains potentially dangerous elements:");
            }

            if (result.ContainsJavaScript)
                summary.AppendLine("  • JavaScript code detected");
            if (result.ContainsEmbeddedFiles)
                summary.AppendLine("  • Embedded files detected");
            if (result.ContainsLaunchActions)
                summary.AppendLine("  • Launch actions detected (can execute programs)");
            if (result.ContainsFormFields)
                summary.AppendLine("  • Form fields detected");
            if (result.ContainsExternalReferences)
                summary.AppendLine("  • External references detected");

            if (result.DetectedThreats.Count > 0)
            {
                summary.AppendLine("\nDetected Threats:");
                foreach (var threat in result.DetectedThreats.Take(5))
                {
                    summary.AppendLine($"  ! {threat}");
                }
                if (result.DetectedThreats.Count > 5)
                {
                    summary.AppendLine($"  ... and {result.DetectedThreats.Count - 5} more");
                }
            }

            if (result.Warnings.Count > 0)
            {
                summary.AppendLine("\nWarnings:");
                foreach (var warning in result.Warnings.Take(5))
                {
                    summary.AppendLine($"  - {warning}");
                }
                if (result.Warnings.Count > 5)
                {
                    summary.AppendLine($"  ... and {result.Warnings.Count - 5} more");
                }
            }

            result.Summary = summary.ToString();
        }
    }
}
