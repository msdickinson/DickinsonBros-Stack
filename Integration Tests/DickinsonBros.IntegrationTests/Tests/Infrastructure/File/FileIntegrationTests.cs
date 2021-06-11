using DickinsonBros.Infrastructure.File.Abstractions;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.File
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "File", Group = "Infrastructure")]
    public class FileIntegrationTests : IFileIntegrationTests
    {
        public readonly IFileService _fileService;

        public FileIntegrationTests
        (
            IFileService fileService
        )
        {
            _fileService = fileService;
        }

        public async Task UpsertAndExistsAndLoadAndDelete_Runs_ExpectedReturnsAndNoThrows(List<string> successLog)
        {
            var filePath = "./";
            var filename = Guid.NewGuid().ToString() + ".txt";
            var fileContent = "Sample File";

            //Upsert (Create)
            await _fileService.UpsertFileAsync(filePath + filename, fileContent, Encoding.ASCII).ConfigureAwait(false);
            successLog.Add($"UpsertFile (Create) Successful."); 
            

            //Upsert (Replace)
            await _fileService.UpsertFileAsync(filePath + filename, fileContent + " Edited", Encoding.ASCII).ConfigureAwait(false);
            successLog.Add($"UpsertFile (Replace) Successful.");

            //Exists
            var exist = _fileService.FileExists(filePath + filename);
            Assert.IsTrue(exist, "FileExists Failed");
            successLog.Add($"FileExists Successful.");

            //Load (Byte[])
            var byteArray = await _fileService.LoadFileAsync(filePath + filename).ConfigureAwait(false);
            Assert.IsNotNull(byteArray, "LoadFile (Byte Array) Failed");
            successLog.Add($"LoadFile (Byte Array) Successful.");

            //Load (String)
            var text = await _fileService.LoadFileAsync(filePath + filename, Encoding.ASCII).ConfigureAwait(false);
            Assert.AreEqual(fileContent + " Edited", text, "LoadFile (String) Failed");
            successLog.Add($"LoadFile (String) Successful.");

            //Delete
            _fileService.DeleteFile(filePath + filename);
            successLog.Add($"DeleteFile Successful.");
        }

    }
}
