using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = GetTypeToNameOf.Test.CSharpCodeFixVerifier<
    GetTypeToNameOf.GetTypeToNameOfAnalyzer,
    GetTypeToNameOf.GetTypeToNameOfCodeFixProvider>;

namespace GetTypeToNameOf.Test
{
    [TestClass]
    public class FoundFixture
    {
        [TestMethod]
        public async Task Class_ThisGetType_Test()
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
        }
    }";

            var fixtest = @"
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
            public void Log() => Console.WriteLine(nameof(MyClass));
        }
    }";

            var expected = VerifyCS.Diagnostic("GetTypeToNameOf").WithLocation(0).WithArguments("this.GetType().Name", "nameof(MyClass)");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Class_GetType_Test()
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
        }
    }";

            var fixtest = @"
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
            public void Log() => Console.WriteLine(nameof(MyClass));
        }
    }";

            var expected = VerifyCS.Diagnostic("GetTypeToNameOf").WithLocation(0).WithArguments("GetType().Name", "nameof(MyClass)");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task NestedClass_ThisGetType_Test()
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
        sealed class MyClass0
        {
            sealed class MyClass
            {
                public void Log() => Console.WriteLine({|#0:this.GetType().Name|});
            }
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        sealed class MyClass0
        {
            sealed class MyClass
            {
                public void Log() => Console.WriteLine(nameof(MyClass));
            }
        }
    }";

            var expected = VerifyCS.Diagnostic("GetTypeToNameOf").WithLocation(0).WithArguments("this.GetType().Name", "nameof(MyClass)");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task NestedClass_GetType_Test()
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
        sealed class MyClass0
        {
            sealed class MyClass
            {
                public void Log() => Console.WriteLine({|#0:GetType().Name|});
            }
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        sealed class MyClass0
        {
            sealed class MyClass
            {
                public void Log() => Console.WriteLine(nameof(MyClass));
            }
        }
    }";

            var expected = VerifyCS.Diagnostic("GetTypeToNameOf").WithLocation(0).WithArguments("GetType().Name", "nameof(MyClass)");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task AnonymousClass_ThisGetType_Test()
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
                var a = new
                {
                    FullName = new Func<string>(() => { return {|#0:this.GetType().Name|}; })
                };
            }
        }
    }";

            var fixtest = @"
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
                var a = new
                {
                    FullName = new Func<string>(() => { return nameof(MyClass); })
                };
            }
        }
    }";

            var expected = VerifyCS.Diagnostic("GetTypeToNameOf").WithLocation(0).WithArguments("this.GetType().Name", "nameof(MyClass)");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task AnonymousClass_GetType_Test()
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
                var a = new
                {
                    FullName = new Func<string>(() => { return {|#0:GetType().Name|}; })
                };
            }
        }
    }";

            var fixtest = @"
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
                var a = new
                {
                    FullName = new Func<string>(() => { return nameof(MyClass); })
                };
            }
        }
    }";

            var expected = VerifyCS.Diagnostic("GetTypeToNameOf").WithLocation(0).WithArguments("GetType().Name", "nameof(MyClass)");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Record_ThisGetType_Test()
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
        sealed record MyClass
        {
            public void Log() => Console.WriteLine({|#0:this.GetType().Name|});
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        sealed record MyClass
        {
            public void Log() => Console.WriteLine(nameof(MyClass));
        }
    }";

            var expected = VerifyCS.Diagnostic("GetTypeToNameOf").WithLocation(0).WithArguments("this.GetType().Name", "nameof(MyClass)");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Record_GetType_Test()
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
        sealed record MyClass
        {
            public void Log() => Console.WriteLine({|#0:GetType().Name|});
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        sealed record MyClass
        {
            public void Log() => Console.WriteLine(nameof(MyClass));
        }
    }";

            var expected = VerifyCS.Diagnostic("GetTypeToNameOf").WithLocation(0).WithArguments("GetType().Name", "nameof(MyClass)");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Struct_ThisGetType_Test()
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
        struct MyClass
        {
            public void Log() => Console.WriteLine({|#0:this.GetType().Name|});
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        struct MyClass
        {
            public void Log() => Console.WriteLine(nameof(MyClass));
        }
    }";

            var expected = VerifyCS.Diagnostic("GetTypeToNameOf").WithLocation(0).WithArguments("this.GetType().Name", "nameof(MyClass)");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Struct_GetType_Test()
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
        struct MyClass
        {
            public void Log() => Console.WriteLine({|#0:GetType().Name|});
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        struct MyClass
        {
            public void Log() => Console.WriteLine(nameof(MyClass));
        }
    }";

            var expected = VerifyCS.Diagnostic("GetTypeToNameOf").WithLocation(0).WithArguments("GetType().Name", "nameof(MyClass)");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

    }
}
