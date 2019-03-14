using System;
using System.Collections.Generic;
using Analyzer.Data.Test.Utils;

namespace Analyzer.Data.Tests.SourceRepositoryV2
{
    public class CommitTestDataBuilder
    {
        private string _authorName;
        private string _email;
        private string _fileName;
        private readonly List<string> _fileContents;
        private DateTime _commitTimestamp;
        private string _commitMessage;
        private string _branch;

        public CommitTestDataBuilder()
        {
            _fileContents = new List<string>();
        }

        public CommitTestDataBuilder With_Author(string name, string email)
        {
            _authorName = name;
            _email = email;
            return this;
        }

        public CommitTestDataBuilder With_File_Name(string fileName)
        {
            _fileName = fileName;
            return this;
        }

        public CommitTestDataBuilder With_File_Content(params string[] contents)
        {
            _fileContents.AddRange(contents);
            return this;
        }

        public CommitTestDataBuilder With_Commit_Timestamp(string timeStamp)
        {
            _commitTimestamp = DateTime.Parse(timeStamp);
            return this;
        }


        public CommitTestDataBuilder With_Commit_Timestamp(DateTime timeStamp)
        {
            _commitTimestamp = timeStamp;
            return this;
        }

        public CommitTestDataBuilder With_Commit_Message(string message)
        {
            _commitMessage = message;
            return this;
        }

        public CommitTestDataBuilder With_Branch(string branch)
        {
            _branch = branch;
            return this;
        }

        public TestCommit Build()
        {
            var result = new TestCommit
            {
                Name = _authorName,
                Email = _email,
                FileName = _fileName,
                Lines = new List<string>(),
                CommitMessage = _commitMessage,
                TimeStamp = _commitTimestamp
            };

            result.Lines.AddRange(_fileContents);

            _fileContents.Clear();

            return result;
        }
    }
}