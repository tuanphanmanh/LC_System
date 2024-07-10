// import { Injectable } from '@angular/core';
// import { PDFDocument, rgb } from 'pdf-lib';

// @Injectable({
//   providedIn: 'root',
// })
// export class PdfMergeService {
//   async mergePDFs(pdfFiles: File[]): Promise<Blob> {
//     const mergedPdf = await PDFDocument.create();
//     for (const pdfFile of pdfFiles) {
//       const pdfBytes = await fetch(URL.createObjectURL(pdfFile)).then((res) =>
//         res.arrayBuffer()
//       );
//       const pdfDoc = await PDFDocument.load(pdfBytes);
//       const copiedPages = await mergedPdf.copyPages(pdfDoc, pdfDoc.getPageIndices());
//       copiedPages.forEach((page) => mergedPdf.addPage(page));
//     }
//     const mergedPdfBytes = await mergedPdf.save();
//     return new Blob([mergedPdfBytes], { type: 'application/pdf' });
//   }
// }
