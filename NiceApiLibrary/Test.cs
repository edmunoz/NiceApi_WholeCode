using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    [TestClass]
    public class Test
    {
        [TestInitialize]
        public void testInit()
        {
        }

        [TestCleanup]
        public void testClean()
        {
        }

        [TestMethod]
        public void DumpPublicKey()
        {
            Assembly a = Assembly.GetAssembly(typeof(NiceApiLibrary.Test));
            string s = a.FullName;
            Debug.WriteLine(s);
        }
    }
}
