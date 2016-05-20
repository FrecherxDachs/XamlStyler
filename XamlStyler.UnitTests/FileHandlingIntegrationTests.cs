﻿using System.IO;
using System.Text;
using NUnit.Framework;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.DocumentManipulation;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.UnitTests
{
    [TestFixture]
    public class FileHandlingIntegrationTests
    {
        [TestCase(0)]
        [TestCase(4)]
        public void TestAttributeIndentationHandling(byte attributeIndentation)
        {
            var stylerOptions = new StylerOptions
            {
                AttributeIndentation = attributeIndentation,
                AttributesTolerance = 0,
                MaxAttributeCharatersPerLine = 80,
                MaxAttributesPerLine = 3,
                PutEndingBracketOnNewLine = true
            };

            DoTestCase(stylerOptions, attributeIndentation);
        }

        [Test]
        public void TestAttributeThresholdHandling()
        {
            var stylerOptions = new StylerOptions
            {
                AttributesTolerance = 0,
                MaxAttributeCharatersPerLine = 80,
                MaxAttributesPerLine = 3,
                PutEndingBracketOnNewLine = true
            };

            DoTest(stylerOptions);
        }

        [Test]
        public void TestAttributeToleranceHandling()
        {
            var stylerOptions = new StylerOptions
            {
                AttributesTolerance = 3,
                RootElementLineBreakRule = LineBreakRule.Always,
            };

            DoTest(stylerOptions);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void TestCommentHandling(byte testNumber)
        {
            var stylerOptions = new StylerOptions
            {
                CommentSpaces = testNumber,
            };

            DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestCommentAtFirstLine()
        {
            DoTest();
        }

        [Test]
        public void TestDefaultHandling()
        {
            DoTest();
        }

        [Test]
        public void TestAttributeSortingOptionHandling()
        {
            var stylerOptions = new StylerOptions
            {
                AttributeOrderingRuleGroups = new[]
                {
                    // Class definition group
                    "x:Class",
                    // WPF Namespaces group
                    "xmlns, xmlns:x",
                    // Other namespace
                    "xmlns:*",
                    // Element key group
                    "Key, x:Key, Uid, x:Uid",
                    // Element name group
                    "Name, x:Name, Title",
                    // Attached layout group
                    "Grid.Column, Grid.ColumnSpan, Grid.Row, Grid.RowSpan, Canvas.Right, Canvas.Bottom, Canvas.Left, Canvas.Top",
                    // Core layout group
                    "MinWidth, MinHeight, Width, Height, MaxWidth, MaxHeight, Margin",
                    // Alignment layout group
                    "Panel.ZIndex, HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment",
                    // Unmatched
                    "*:*, *",
                    // Miscellaneous/Other attributes group
                    "Offset, Color, TargetName, Property, Value, StartPoint, EndPoint, PageSource, PageIndex",
                    // Blend related group
                    "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText",
                }
            };

            DoTest(stylerOptions);
        }

        [Test]
        public void TestxBindSplitting()
        {
            var stylerOptions = new StylerOptions
            {
                NoNewLineMarkupExtensions = "x:Bind"
            };

            DoTest(stylerOptions);
        }

        [Test]
        public void TestBindingSplitting()
        {
            var stylerOptions = new StylerOptions
            {
                NoNewLineMarkupExtensions = "x:Bind, Binding"
            };

            DoTest(stylerOptions);
        }

        [TestCase(false,2)]
        [TestCase(false,4)]
        [TestCase(true,2)]
        [TestCase(true,4)]
        public void TestMarkupExtensionHandling(bool indentWithTabs, int tabSize)
        {
            var stylerOptions = new StylerOptions
            {
                FormatMarkupExtension = true,
                IndentWithTabs = indentWithTabs,
                IndentSize = tabSize,
                AttributeIndentationStyle = AttributeIndentationStyle.Mixed,
            };

            DoTestCase(stylerOptions, $"{tabSize}_{(indentWithTabs ? "tabs" : "spaces")}");
        }

        [Test]
        public void TestMarkupWithAttributeNotOnFirstLine()
        {
            var stylerOptions = new StylerOptions
            {
                KeepFirstAttributeOnSameLine = false,
                AttributesTolerance = 1
            };

            DoTest(stylerOptions);
        }

        [Test]
        public void TestNoContentElementHandling()
        {
            DoTest();
        }

        [Test]
        public void TestTextOnlyContentElementHandling()
        {
            DoTest();
        }


        [Test]
        public void TestGridChildrenHandling()
        {
            DoTest();
        }

        [Test]
        public void TestNestedGridChildrenHandling()
        {
            DoTest();
        }

        [Test]
        public void TestCanvasChildrenHandling()
        {
            DoTest();
        }

        [Test]
        public void TestNestedCanvasChildrenHandling()
        {
            DoTest();
        }

        [Test]
        public void TestNestedPropertiesAndChildrenHandling()
        {
            DoTest();
        }

        [Test]
        public void TestAttributeOrderRuleGroupsOnSeparateLinesHandling()
        {
            var stylerOptions = new StylerOptions
            {
                PutAttributeOrderRuleGroupsOnSeparateLines = true,
                MaxAttributesPerLine = 3,
            };

            DoTest(stylerOptions);
        }

        [TestCase(ReorderSettersBy.Property)]
        [TestCase(ReorderSettersBy.TargetName)]
        [TestCase(ReorderSettersBy.TargetNameThenProperty)]
        public void TestReorderSetterHandling(ReorderSettersBy reorderSettersBy)
        {
            var stylerOptions = new StylerOptions
            {
                ReorderSetters = reorderSettersBy,
            };

            DoTestCase(stylerOptions, reorderSettersBy);
        }

        [TestCase(1, true)]
        [TestCase(2, false)]
        public void TestClosingElementHandling(int testNumber, bool spaceBeforeClosingSlash)
        {
            var stylerOptions = new StylerOptions
            {
                SpaceBeforeClosingSlash = spaceBeforeClosingSlash
            };

            DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestCDATAHandling()
        {
            DoTest();
        }

        [Test]
        public void TestXmlSpaceHandling()
        {
            DoTest();
        }

        [TestCase(ThicknessStyle.None)]
        [TestCase(ThicknessStyle.Comma)]
        [TestCase(ThicknessStyle.Space)]
        public void TestThicknessHandling(ThicknessStyle thicknessStyle)
        {
            var stylerOptions = new StylerOptions
            {
                ThicknessStyle = thicknessStyle
            };

            DoTestCase(stylerOptions, thicknessStyle);
        }

        [TestCase(1, LineBreakRule.Default)]
        [TestCase(2, LineBreakRule.Always)]
        [TestCase(3, LineBreakRule.Never)]
        public void TestRootHandling(int testNumber, LineBreakRule lineBreakRule)
        {
            var stylerOptions = new StylerOptions
            {
                AttributesTolerance = 3,
                MaxAttributesPerLine = 4,
                PutAttributeOrderRuleGroupsOnSeparateLines = true,
                RootElementLineBreakRule = lineBreakRule,
            };

            DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestRunHandling()
        {
            DoTest();
        }

        private void DoTest([System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            DoTest(new StylerOptions(), callerMemberName);
        }

        private void DoTest(StylerOptions stylerOptions, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            DoTest(stylerOptions, Path.Combine("TestFiles", callerMemberName), null);
        }

        private void DoTestCase<T>(StylerOptions stylerOptions, T testIdentifier, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            DoTest(stylerOptions, Path.Combine("TestFiles", callerMemberName), testIdentifier.ToString());
        }

        /// <summary>
        /// Style input document and verify output against expected  
        /// </summary>
        /// <param name="stylerOptions"></param>
        /// <param name="testFileBaseName"></param>
        /// <param name="expectedSuffix"></param>
        private static void DoTest(StylerOptions stylerOptions, string testFileBaseName, string expectedSuffix)
        {
            var stylerService = new StylerService(stylerOptions);
            
            var testFileResultBaseName = expectedSuffix != null ? testFileBaseName + "_" + expectedSuffix : testFileBaseName;

            // Excercise stylerService using supplied test xaml data
            string actualOutput = stylerService.StyleDocument(File.ReadAllText(testFileBaseName + ".testxaml"));

            // Write output to ".actual" file for further investigation
            File.WriteAllText(testFileResultBaseName + ".actual", actualOutput, Encoding.UTF8);

            // Check result
            Assert.That(actualOutput, Is.EqualTo(File.ReadAllText(testFileResultBaseName + ".expected")));
        }
    }
}
