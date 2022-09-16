using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = GetTypeToNameOf.Test.CSharpCodeFixVerifier<
    GetTypeToNameOf.GetTypeToNameOfAnalyzer,
    GetTypeToNameOf.GetTypeToNameOfCodeFixProvider>;

namespace GetTypeToNameOf.Test
{
    [TestClass]
    public class NotFoundFixture
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task Empty_Test()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task FakeMethodSameClass_ThisGetType_Test()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        sealed class MyClass
        {
            public void Log() => Console.WriteLine({|#0:this.GetType().Name|});

            public FakeStruct GetType() => new FakeStruct { Name = ""fake"" };
        }

        struct FakeStruct
        {
            public string Name;
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task FakeMethodSameClass_GetType_Test()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        sealed class MyClass
        {
            public void Log() => Console.WriteLine({|#0:GetType().Name|});

            public FakeStruct GetType() => new FakeStruct { Name = ""fake"" };
        }

        struct FakeStruct
        {
            public string Name;
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }


        [TestMethod]
        public async Task FakeMethodBaseClass_ThisGetType_Test()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        sealed class MyClass : MyBaseClass
        {
            public void Log() => Console.WriteLine({|#0:this.GetType().Name|});
        }

        class MyBaseClass
        {
            public FakeStruct GetType() => new FakeStruct { Name = ""fake"" };
        }

        struct FakeStruct
        {
            public string Name;
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task FakeMethodBaseClass_GetType_Test()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        sealed class MyClass : MyBaseClass
        {
            public void Log() => Console.WriteLine({|#0:GetType().Name|});
        }

        class MyBaseClass
        {
            public FakeStruct GetType() => new FakeStruct { Name = ""fake"" };
        }

        struct FakeStruct
        {
            public string Name;
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task OtherClassMethod_GetType_Test()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        sealed class MyClass
        {
            public void Log()
            {
                var s = ""123"";
                Console.WriteLine({|#0:s.GetType().Name|});
            }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task OtherStaticClassMethod_GetType_Test()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        sealed class MyClass
        {
            public void Log()
            {
                Console.WriteLine({|#0:OtherClass.GetType().Name|});
            }
        }

        static class OtherClass
        {
            public static FakeStruct GetType() => new FakeStruct { Name = ""fake"" };
        }

        struct FakeStruct
        {
            public string Name;
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
