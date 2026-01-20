using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using UglyToad.PdfPig;
namespace SPlus.Helper
{
    public class MimeTypes
    {

        [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        static extern int FindMimeFromData(IntPtr pBC,
        [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.I1, SizeParamIndex=3)]
        byte[] pBuffer,
        int cbSize,
        [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
        int dwMimeFlags,
        out IntPtr ppwzMimeOut,
        int dwReserved);



        public static bool CheckRealMimeType(byte[] buffer)
        {
            try
            {

                UInt32 mimeType = default(UInt32);
                IntPtr mimeTypePtr = new IntPtr(mimeType);
                FindMimeFromData(new IntPtr(0), null, buffer, 256, null, 0, out mimeTypePtr, 0);


                string mime = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
                if (string.IsNullOrWhiteSpace(mime))
                    mime = "application/octet-stream";
                bool isAllowed = CheckAllowed(mime);
                return isAllowed;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static bool CheckAllowed(string MimeType)
        {
            bool isAllowed = false;

            switch (MimeType)
            {
                case "text/plain":
                    isAllowed = true;
                    break;
                case "application/msword":
                    isAllowed = true;
                    break;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    isAllowed = true;
                    break;
                case "application/vnd.ms-excel":
                    isAllowed = true;
                    break;
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    isAllowed = true;
                    break;
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.template":
                    isAllowed = true;
                    break;
                case "application/vnd.ms-excel.sheet.macroEnabled.12":
                    isAllowed = true;
                    break;
                case "application/vnd.ms-excel.template.macroEnabled.12":
                    isAllowed = true;
                    break;
                case "application/vnd.ms-excel.addin.macroEnabled.12":
                    isAllowed = true;
                    break;
                case "application/vnd.ms-excel.sheet.binary.macroEnabled.12":
                    isAllowed = true;
                    break;
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                    isAllowed = true;
                    break;
                case "application/vnd.openxmlformats-officedocument.presentationml.slideshow":
                    isAllowed = true;
                    break;
                case "application/pdf":
                    isAllowed = true;
                    break;
                case "image/jpeg":
                    isAllowed = true;
                    break;
                case "image/jpg":
                    isAllowed = true;
                    break;
                case "image/pjpeg":
                    isAllowed = true;
                    break;
                case "image/x-png":
                    isAllowed = true;
                    break;
                case "application/x-zip-compressed":
                    isAllowed = true;
                    break;
                case "application/octet-stream":
                    isAllowed = true;
                    break;
                case "text/xml":
                    isAllowed = true;
                    break;
                case "image/svg+xml":
                    isAllowed = true;
                    break;
            }
            return isAllowed;
        }

        public static bool Verify(byte[] bytes)
        {
            var SignatureLength = Signatures.Max(m => m.Signature.Length);
            var headerBytes = bytes.Take(SignatureLength).ToList();

            string content = System.Text.Encoding.UTF8.GetString(bytes.Take(1024).ToArray());
            if (content.Contains("<svg"))
            {
                return true;
            }
            if (content.StartsWith("From:") ||
   content.StartsWith("Return-Path:") ||
   content.StartsWith("Delivered-To:"))
            {
                // valid EML
                return true;
            }

            string[] emlHeaders = new[]
{
    "From:",
    "Return-Path:",
    "Delivered-To:",
    "MIME-Version:",
    "Subject:",
    "Date:",
    "Content-Type:",
    "Message-ID:"
};

            if (emlHeaders.Any(h => content.StartsWith(h, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }



            return Signatures.Any(s =>
                headerBytes.Take(s.Signature.Length)
                    .SequenceEqual(s.Signature));

        }

        private static List<FileType> Signatures = new List<FileType>() {

            new FileType(){Name = "", Signature = new byte[]{ 0x25, 0x50, 0x44, 0x46, 0x2D } },

            new FileType(){Name = "PDF", Signature = new byte[]{ 0x25, 0x50, 0x44, 0x46, 0x2D } },

            new FileType(){Name = "JPEG/JPEG/JPG", Signature = new byte[]{ 0xFF, 0xD8, 0xFF, 0xDB } },
            new FileType(){Name = "JPEG/JPEG/JPG", Signature = new byte[]{ 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x00, 0x01 } },
            new FileType(){Name = "JPEG/JPEG/JPG", Signature = new byte[]{ 0xFF, 0xD8, 0xFF, 0xEE } },
            new FileType(){Name = "JPEG/JPEG/JPG", Signature = new byte[]{ 0xFF, 0xD8, 0xFF, 0xE1 } },
            new FileType(){Name = "JPEG/JPEG/JPG", Signature = new byte[]{ 0xFF, 0xD8, 0xFF, 0xE0 } },
            new FileType(){Name = "JPEG/JPEG/JPG", Signature = new byte[]{ 0xFF, 0xD8, 0xFF, 0xE2 } },
            new FileType(){Name = "PNG", Signature = new byte[]{ 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },


            new FileType(){Name = "DOCX/XLSX/PPTX", Signature = new byte[]{ 0x50, 0x4B, 0x03, 0x04} },
            new FileType(){Name = "DOCX/XLSX/PPTX", Signature = new byte[]{ 0x50, 0x4B, 0x05, 0x06 } },
            new FileType(){Name = "DOCX/XLSX/PPTX", Signature = new byte[]{ 0x50, 0x4B, 0x07, 0x08 } },
            new FileType(){Name = "DOC/XLS/PPT/MPP", Signature = new byte[]{ 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } },

            new FileType(){Name = "txt", Signature = new byte[]{ 0xEF, 0xBB,0xBF} },
            new FileType(){Name = "txt", Signature = new byte[]{ 0xFF, 0xFE} },
            new FileType(){Name = "txt", Signature = new byte[]{ 0xFE, 0xFF} },
            new FileType(){Name = "txt", Signature = new byte[]{ 0xFF, 0xFE,0x00,0x00} },
            new FileType(){Name = "txt", Signature = new byte[]{ 0x00,0x00,0xFE, 0xFF} },
            new FileType(){Name = "txt", Signature = new byte[]{ 0x0E,0xFE, 0xFF} },


            new FileType(){Name = "xml", Signature = new byte[]{ 0x3C, 0x3F,0x78,0x6D,0x6C,0x20} },
            new FileType(){Name = "xml", Signature = new byte[]{0x3C, 0x00,0x3F,0x00,0x78,0x00,0x6D,0x00}  },
            new FileType(){Name = "xml", Signature = new byte[]{ 0x6C, 0x00,0x20}  },
            new FileType(){Name = "xml", Signature = new byte[]{ 0x00, 0x3C,0x00,0x3F,0x00,0x78,0x00,0x6D}  },
            new FileType(){Name = "xml", Signature = new byte[]{ 0x00, 0x6C,0x00,0x20}  },
            new FileType(){Name = "xml", Signature = new byte[]{0x3C, 0x00,0x00,0x00,0x3F,0x00,0x00,0x00} },
            new FileType(){Name = "xml", Signature = new byte[]{0x78, 0x00,0x00,0x00,0x6D,0x00,0x00,0x00} },
            new FileType(){Name = "xml", Signature = new byte[]{0x6C, 0x00,0x00,0x00,0x20,0x00,0x00,0x00} },
            new FileType(){Name = "xml", Signature = new byte[]{0x00, 0x00,0x00,0x3C,0x00,0x00,0x00,0x3F} },
            new FileType(){Name = "xml", Signature = new byte[]{0x00, 0x00,0x00,0x78,0x00,0x00,0x00,0x6D} },
            new FileType(){Name = "xml", Signature = new byte[]{0x00, 0x00,0x00,0x6C,0x00,0x00,0x00,0x20} },
            new FileType(){Name = "xml", Signature = new byte[]{0x4C, 0x6F,0xA7,0x94,0x93,0x40} },
            new FileType(){Name = "xml", Signature = new byte[]{0x45, 0x6C,0x66,0x46,0x69,0x6C,0x65} },
            new FileType(){Name = "SVG", Signature = new byte[]{ 0x3C, 0x73, 0x76, 0x67, 0x20 } }, // Matches "<svg "

            new FileType(){Name = "zip", Signature = new byte[]{ 0x50, 0x4B,0x03,0x04} },
            new FileType(){Name = "zip", Signature = new byte[]{ 0x50,0x4B,0x05, 0x06} },
            new FileType(){Name = "zip", Signature = new byte[]{ 0x50,0x4B, 0x07,0x08} },


            new FileType(){Name = "XLSB/MSG", Signature = new byte[]{ 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } },
new FileType(){Name = "EML", Signature = new byte[]{ 0x46, 0x72, 0x6F, 0x6D, 0x3A } },
new FileType(){Name = "VCF", Signature = new byte[]{ 0x42, 0x45, 0x47, 0x49, 0x4E } },
new FileType(){Name = "VCS", Signature = new byte[]{0x42, 0x45, 0x47, 0x49, 0x4E, 0x3A, 0x56, 0x43 } },

        };


        /// <summary>
        /// Scans the file for potentially malicious content.
        /// </summary>
        /// <param name="filePath">The file path to scan.</param>
        /// <returns>True if malicious content is found; otherwise false.</returns>
        //public static async Task<(bool isClean, bool containsMaliciousContent)> ValidateAndSanitizePdf(byte[] pdfBytes)
        //{
        //    bool isClean = true;
        //    bool containsMaliciousContent = false;

        //    try
        //    {
        //        // Step 1: Validate the PDF content for malicious scripts
        //        using (var memoryStream = new MemoryStream(pdfBytes))
        //        {
        //            using (var pdfDocument = PdfDocument.Open(memoryStream))
        //            {
        //                foreach (var page in pdfDocument.GetPages())
        //                {
        //                    var pageText = page.Text;
        //                    if (ContainsMaliciousContent(pageText))
        //                    {
        //                        containsMaliciousContent = true;
        //                        isClean = false;
        //                        // Sanitize (this can be extended to remove the malicious parts)
        //                        Console.WriteLine("Malicious content found, sanitizing...");
        //                    }
        //                }
        //            }
        //        }

        //        return (isClean, containsMaliciousContent);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error processing PDF: {ex.Message}");
        //        return (false, containsMaliciousContent);
        //    }
        //}

        //// Method to detect if the PDF text contains malicious patterns
        //private static bool ContainsMaliciousContent(string content)
        //{
        //    foreach (string pattern in maliciousPatterns)
        //    {
        //        if (Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline))
        //        {
        //            return true; // Found malicious content
        //        }
        //    }
        //    return false;
        //}




        //public static bool IsValidImage(byte[] fileBytes)
        //{
        //    if (fileBytes == null || fileBytes.Length < 4)
        //        return false; // File too small to be a valid image

        //    // Check for JPEG magic number (0xFF 0xD8 0xFF)
        //    if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8 && fileBytes[2] == 0xFF)
        //        return true; // Valid JPEG image

        //    // Check for PNG magic number (0x89 0x50 0x4E 0x47 -> .PNG)
        //    if (fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E && fileBytes[3] == 0x47)
        //        return true; // Valid PNG image

        //    // Check for GIF magic number (GIF87a or GIF89a -> 'G' 'I' 'F')
        //    if (fileBytes[0] == 0x47 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46)
        //        return true; // Valid GIF image

        //    // Check for BMP magic number (0x42 0x4D -> 'BM')
        //    if (fileBytes[0] == 0x42 && fileBytes[1] == 0x4D)
        //        return true; // Valid BMP image

        //    // Check for TIFF magic number (0x49 0x49 or 0x4D 0x4D -> II or MM)
        //    if ((fileBytes[0] == 0x49 && fileBytes[1] == 0x49) || (fileBytes[0] == 0x4D && fileBytes[1] == 0x4D))
        //        return true; // Valid TIFF image

        //    return false; // Not a recognized image format
        //}


        //private static readonly string[] maliciousPatterns = new string[]
        //{
        //    //// JavaScript-related attacks
        //    @"<script.*?>.*?</script>",      // Matches <script> tags
        //    //@"(?i)onerror\s*=\s*",           // Matches JavaScript onerror event
        //    //@"(?i)onclick\s*=\s*",           // Matches JavaScript onclick event
        //    //@"(?i)onload\s*=\s*",            // Matches JavaScript onload event
        //    @"(?i)alert\(",                  // Matches alert()
        //    @"(?i)eval\(",                   // Matches eval() function
        //    //@"(?i)document\.cookie",          // Matches JavaScript document.cookie access
        //    //@"(?i)window\.location",          // Matches JavaScript window.location access
        //    //@"(?i)javascript:",              // Matches javascript: URI schemes
        //    //@"(?i)<iframe.*?>.*?</iframe>",  // Matches iframe tags
        //    @"(?i)<img.*?src\s*=\s*javascript:",  // Matches JavaScript in image src
        //    //@"(?i)<object.*?>.*?</object>",  // Matches object tags

        //    //// Cross-site scripting (XSS) patterns
        //    //@"(?i)<script>",                 // Matches open script tag
        //    //@"(?i)<.*?onmouseover\s*=\s*",   // Matches onmouseover event for XSS
        //    //@"(?i)<.*?onfocus\s*=\s*",       // Matches onfocus event for XSS
        //    //@"(?i)<.*?onerror\s*=\s*",       // Matches onerror event for XSS
        //    //@"(?i)<.*?onsubmit\s*=\s*",      // Matches onsubmit event for XSS
        //    //@"(?i)<.*?onchange\s*=\s*",      // Matches onchange event for XSS
        //    //@"(?i)<.*?onkeydown\s*=\s*",     // Matches onkeydown event for XSS
        //    //@"(?i)<svg.*?>.*?</svg>",        // Matches <svg> tag for XSS
        //    //@"(?i)<img\s+src\s*=\s*[^>]*>",  // Matches <img src=""> for XSS

        //    @"<script.*?>.*?</script>",  // JavaScript code inside PDFs
        //    @"(?i)eval\(",               // JavaScript eval function
        //    @"(?i)onerror\s*=\s*",       // JavaScript onerror event
        //    @"(?i)alert\(",              // JavaScript alert function
        //    @"(?i)wscript",              // Windows Script Host commands

        //};





    }

    public class FileType
    {
        public string Name { get; set; }
        public byte[] Signature { get; set; }
    }
}
