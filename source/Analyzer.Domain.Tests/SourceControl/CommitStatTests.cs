using System;
using System.Collections.Generic;
using Analyzer.Domain.Developer;
using Analyzer.Domain.SourceControl;
using Analyzer.Domain.SourceControl.Stats;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Domain.Tests.SourceControl
{
    [TestFixture]
    public class CommitStatTests
    {
        [TestFixture]
        class Author_Stats
        {
            [Test]
            public void Expect_Author_On_Commit_Returned()
            {
                //---------------Arrange------------------
                var commit = new Commit
                {
                    Author = new Author
                    {
                        Name = "T-rav",
                        Emails = new List<string> { "t@stoneage.com" }
                    },
                    When = DateTime.Now,
                    Patch = new List<Patch>
                    {
                        new Patch
                        {
                            Contents = "a\nb",
                            LinesAdded = 2,
                            LinesRemoved = 0
                        }
                    }
                };
                var sut = new CommitStat(commit);
                //---------------Act----------------------
                var actual = sut.Author;
                //---------------Assert-------------------
                actual.Should().BeEquivalentTo(commit.Author);
            }
        }

        [TestFixture]
        class When
        {
            [Test]
            public void Expect_When_On_Commit_Returned()
            {
                //---------------Arrange------------------
                var commit = new Commit
                {
                    Author = new Author
                    {
                        Name = "T-rav",
                        Emails = new List<string> { "t@stoneage.com" }
                    },
                    When = DateTime.Now,
                    Patch = new List<Patch>
                    {
                        new Patch
                        {
                            Contents = "a\nb",
                            LinesAdded = 2,
                            LinesRemoved = 0
                        }
                    }
                };
                var sut = new CommitStat(commit);
                //---------------Act----------------------
                var actual = sut.When;
                //---------------Assert-------------------
                actual.Should().Be(commit.When);
            }
        }


        [TestFixture]
        class Lines_Stats
        {
            [Test]
            public void GivenLinesAdded_Expect_LinesAdded_On_Commit_Returned()
            {
                //---------------Arrange------------------
                var commit = new Commit
                {
                    Author = new Author
                    {
                        Name = "T-rav",
                        Emails = new List<string> { "t@stoneage.com" }
                    },
                    When = DateTime.Now,
                    Patch = new List<Patch>
                    {
                        new Patch
                        {
                            Contents = "a\nb",
                            LinesAdded = 2,
                            LinesRemoved = 0
                        }
                    }
                };
                var sut = new CommitStat(commit);
                //---------------Act----------------------
                var actual = sut.LinesAdded;
                //---------------Assert-------------------
                actual.Should().Be(commit.LinesAdded);
            }

            [Test]
            public void GivenLinesRemoved_Expect_LinesRemoved_On_Commit_Returned()
            {
                //---------------Arrange------------------
                var commit = new Commit
                {
                    Author = new Author
                    {
                        Name = "T-rav",
                        Emails = new List<string> { "t@stoneage.com" }
                    },
                    When = DateTime.Now,
                    Patch = new List<Patch>
                    {
                        new Patch
                        {
                            Contents = "a\nb",
                            LinesAdded = 2,
                            LinesRemoved = 3
                        }
                    }
                };
                var sut = new CommitStat(commit);
                //---------------Act----------------------
                var actual = sut.LinesRemoved;
                //---------------Assert-------------------
                actual.Should().Be(commit.LinesRemoved);
            }
        }

        [TestFixture]
        class Churn
        {
            [Test]
            public void GivenNoLinesAddedOrRemoved_Expect_Zero_Churn()
            {
                //---------------Arrange------------------
                var commit = new Commit
                {
                    Author = new Author
                    {
                        Name = "T-rav",
                        Emails = new List<string> {"t@stoneage.com"}
                    },
                    When = DateTime.Now,
                    Patch = new List<Patch>
                    {
                        new Patch
                        {
                            Contents = string.Empty,
                            LinesAdded = 0,
                            LinesRemoved = 0
                        }
                    }
                };
                var sut = new CommitStat(commit);
                //---------------Act----------------------
                var actual = sut.Churn();
                //---------------Assert-------------------
                actual.Should().Be(0);
            }

            [Test]
            public void GivenOnlyLinesRemoved_Expect_Zero_Churn()
            {
                //---------------Arrange------------------
                var commit = new Commit
                {
                    Author = new Author
                    {
                        Name = "T-rav",
                        Emails = new List<string> { "t@stoneage.com" }
                    },
                    When = DateTime.Now,
                    Patch = new List<Patch>
                    {
                        new Patch
                        {
                            Contents = string.Empty,
                            LinesAdded = 0,
                            LinesRemoved = 3
                        }
                    }
                };
                var sut = new CommitStat(commit);
                //---------------Act----------------------
                var actual = sut.Churn();
                //---------------Assert-------------------
                actual.Should().Be(0);
            }

            [Test]
            public void GivenOnlyLinesAdded_Expect_Zero_Churn()
            {
                //---------------Arrange------------------
                var commit = new Commit
                {
                    Author = new Author
                    {
                        Name = "T-rav",
                        Emails = new List<string> { "t@stoneage.com" }
                    },
                    When = DateTime.Now,
                    Patch = new List<Patch>
                    {
                        new Patch
                        {
                            Contents = string.Empty,
                            LinesAdded = 5,
                            LinesRemoved = 0
                        }
                    }
                };
                var sut = new CommitStat(commit);
                //---------------Act----------------------
                var actual = sut.Churn();
                //---------------Assert-------------------
                actual.Should().Be(0);
            }

            [Test]
            public void GivenLinesAddedAndRemoved_Expect_Churn()
            {
                //---------------Arrange------------------
                var commit = new Commit
                {
                    Author = new Author
                    {
                        Name = "T-rav",
                        Emails = new List<string> { "t@stoneage.com" }
                    },
                    When = DateTime.Now,
                    Patch = new List<Patch>
                    {
                        new Patch
                        {
                            Contents = string.Empty,
                            LinesAdded = 5,
                            LinesRemoved = 3
                        }
                    }
                };
                var sut = new CommitStat(commit);
                //---------------Act----------------------
                var actual = sut.Churn();
                //---------------Assert-------------------
                actual.Should().Be(0.6);
            }
        }

        [TestFixture]
        class Impact_Stats
        {
            [Test]
            public void Given_No_Change_Expect_Zero_Impact()
            {
                //---------------Arrange------------------
                var commit = new Commit
                {
                    Author = new Author
                    {
                        Name = "T-rav",
                        Emails = new List<string> { "t@stoneage.com" }
                    },
                    When = DateTime.Now,
                    Patch = new List<Patch>
                    {
                        new Patch
                        {
                            Contents = string.Empty,
                            LinesAdded = 0,
                            LinesRemoved = 0
                        }
                    }
                };
                var sut = new CommitStat(commit);
                //---------------Act----------------------
                var actual = sut.Impact(new List<string>(),false);
                //---------------Assert-------------------
                actual.Should().Be(0);
            }

            [Test]
            public void Given_One_New_Line_Added_In_One_New_File_Expect_New_Code_Base_Impact()
            {
                //---------------Arrange------------------
                var commit = new Commit
                {
                    Author = new Author
                    {
                        Name = "T-rav",
                        Emails = new List<string> { "t@stoneage.com" }
                    },
                    When = DateTime.Now,
                    Patch = new List<Patch>
                    {
                        new Patch
                        {
                            Contents = "@@ 1,0 1,0 @@ namespace Test.Namespace \na",
                            LinesAdded = 1,
                            LinesRemoved = 0,
                            ChangeType = ChangeType.Added
                        }
                    }
                };
                var sut = new CommitStat(commit);
                //---------------Act----------------------
                var actual = sut.Impact(new List<string>(), false);
                //---------------Assert-------------------
                var expectedImpact = 0.001;
                actual.Should().Be(expectedImpact);
            }

            [Test]
            public void Given_One_New_Line_Added_In_One_Old_File_Expect_Old_Code_Base_Impact()
            {
                //---------------Arrange------------------
                var commit = new Commit
                {
                    Author = new Author
                    {
                        Name = "T-rav",
                        Emails = new List<string> { "t@stoneage.com" }
                    },
                    When = DateTime.Now,
                    Patch = new List<Patch>
                    {
                        new Patch
                        {
                            Contents = "@@ 1,0 1,0 @@ namespace Test.Namespace \na",
                            LinesAdded = 1,
                            LinesRemoved = 0,
                            ChangeType = ChangeType.Modified
                        }
                    }
                };
                var sut = new CommitStat(commit);
                //---------------Act----------------------
                var actual = sut.Impact(new List<string>(), false);
                //---------------Assert-------------------
                var expectedImpact = 0.0015;
                actual.Should().Be(expectedImpact);
            }
        }
    }
}
