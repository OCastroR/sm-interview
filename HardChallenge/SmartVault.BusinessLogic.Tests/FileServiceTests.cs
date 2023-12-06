using Moq;
using NUnit.Framework;
using SmartVaul.Repository.Interfaces;
using SmartVault.BusinessLogic.Interfaces;
using System.Collections.Generic;

namespace SmartVault.BusinessLogic.Tests
{
    [TestFixture()]
    public class FileServiceTests
    {
        private Mock <IDocumentRespository> _mockDocumentRepository;
        private Mock<IFileManagement> _mockFileManagement;

        [SetUp]
        public void Setup()
        {
            _mockDocumentRepository = new Mock<IDocumentRespository>();
            _mockFileManagement = new Mock<IFileManagement>();
        }

        [Test]
        public void should_get_all_files_size() 
        {
            //Arrange

            var filePathsList = new List<string>();
            filePathsList.Add("path1");
            filePathsList.Add("path2");

            _mockDocumentRepository.Setup(repo => repo.GetAllFilesPath()).Returns(filePathsList);
            _mockFileManagement.Setup( fm => fm.GetFileLenght(It.IsAny<string>())).Returns(5);

            var fileService = new FileService(_mockFileManagement.Object, _mockDocumentRepository.Object);

            //Act

            var filesLength = fileService.GetAllFileSizesInBytes();

            //Assert

            Assert.That(filesLength, Is.EqualTo(10));
        }

        [Test]
        public void should_evaluate_only_third_document()
        {
            //Arrange


            var filePathsList = new List<(int id, string filePath)>
            {
                (1, "path1"),
                (2, "path2"),
                (3, "path3")
            };
            _mockDocumentRepository.Setup(repo => repo.GetFilePath(It.IsAny<int>())).Returns(filePathsList);
            _mockFileManagement.Setup(fm => fm.ReadAll(It.IsAny<string>())).Returns("test text");
            _mockFileManagement.Setup(fm => fm.WriteAll(It.IsAny<string>(), It.IsAny<string>()));

            var fileService = new FileService(_mockFileManagement.Object, _mockDocumentRepository.Object);

            //Act

            fileService.WriteEveryThirdFileToFile(1,"test");

            //Assert
            Mock.Get(_mockFileManagement.Object).Verify(x =>x.WriteAll(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void should_write_only_third_document_if_contains_keyword()
        {
            //Arrange


            var filePathsList = new List<(int id, string filePath)>
            {
                (1, "path1"),
                (2, "path2"),
                (3, "path3"),
                (4, "path4"),
                (5, "path5"),
                (6, "path6"),
            };
            _mockDocumentRepository.Setup(repo => repo.GetFilePath(It.IsAny<int>())).Returns(filePathsList);
            _mockFileManagement.Setup(fm => fm.ReadAll(It.IsAny<string>())).Returns("wrong text");
            _mockFileManagement.Setup(fm => fm.ReadAll("path6")).Returns("test text");
            _mockFileManagement.Setup(fm => fm.WriteAll(It.IsAny<string>(), It.IsAny<string>()));

            var fileService = new FileService(_mockFileManagement.Object, _mockDocumentRepository.Object);

            //Act

            fileService.WriteEveryThirdFileToFile(1, "test");

            //Assert
            Mock.Get(_mockFileManagement.Object).Verify(x => x.WriteAll(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
