using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GroceryListApi.Tests.Infrastructure;

[PublicAPI]
public class AlphabeticalTestCaseOrderer : ITestCaseOrderer
{
    private IMessageSink _diagnosticMessageSink;
        
    public AlphabeticalTestCaseOrderer(IMessageSink diagnosticMessageSink)
        => _diagnosticMessageSink = diagnosticMessageSink;

    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var result = testCases.ToList();
        result.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
        return result;
    }
}