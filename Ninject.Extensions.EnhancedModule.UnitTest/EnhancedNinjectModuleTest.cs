//------------------------------------------------------------------------------
// <copyright file="EnhancedNinjectModuleTest.cs" company="Q_Division">
//	Copyright (c) 2018 Q_Division. All rights reserved.
//
//  Licensed under MIT License (see LICENSE.txt for full license)
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Moq;
using Ninject.Modules;
using NUnit.Framework;

namespace Ninject.Extensions.EnhancedModule.UnitTest
{
	/// <summary>
	/// Tests for EnhancedNinjectModule
	/// </summary>
	[TestFixture]
	public class EnhancedNinjectModuleTest
	{
		#region Test Class
		/// <summary>
		/// Test Class
		/// Exposes protected properties as use case is a derived class
		/// </summary>
		public class TestEnhancedNinjectModule : EnhancedNinjectModule
		{
			public List<NinjectModule> ExposedModuleDependencies => ModuleDependencies;

			public override void Load()
			{
				
			}

			public void ExposedAddDependency(NinjectModule dependency)
			{
				AddDependency(dependency);
			}

			public void ExposedLoadDependencies()
			{
				LoadDependencies();
			}
		}
		#endregion

		#region Test Dependancy
		/// <summary>
		/// Test Class for Dependency
		/// </summary>
		public class TestDependency : NinjectModule
		{
			public override void Load()
			{

			}
		}
		#endregion

		/// <summary>
		/// Tests Constructor Defaults
		/// Makes sure we have the ModuleDependencies list
		/// </summary>
		[Test]
		public void ConstructorDefaults()
		{
			//Execute
			var module = new TestEnhancedNinjectModule();

			//Check
			Assert.That(module.ExposedModuleDependencies, Is.Not.Null, "Null Check");
			Assert.That(module.ExposedModuleDependencies.Count, Is.Zero, "Count");
		}

		/// <summary>
		/// Tests adding a new dependency
		/// </summary>
		[Test]
		public void AddDependancyNew()
		{
			//Setup
			var testdependency = new TestDependency();
			var module = new TestEnhancedNinjectModule();

			var mockkernel = Mock.Of<IKernel>(k => k.HasModule(testdependency.GetType().FullName) == false);
			module.OnLoad(mockkernel);

			//Execute
			module.ExposedAddDependency(testdependency);

			//Check
			Assert.That(module.ExposedModuleDependencies.Contains(testdependency), Is.True);
		}

		/// <summary>
		/// Tests adding a dependency that is already loaded
		/// </summary>
		[Test]
		public void AddDependancyExisting()
		{
			//Setup
			var testdependency = new TestDependency();
			var module = new TestEnhancedNinjectModule();

			var mockkernel = new Mock<IKernel>();
			mockkernel.Setup(k => k.HasModule(testdependency.GetType().FullName)).Returns(false);
			module.OnLoad(mockkernel.Object);

			var origtestdependency = new TestDependency();
			module.ExposedAddDependency(origtestdependency);
			mockkernel.Setup(k => k.HasModule(testdependency.GetType().FullName)).Returns(true);

			//Execute
			module.ExposedAddDependency(testdependency);

			//Check
			Assert.That(module.ExposedModuleDependencies.Contains(testdependency), Is.False);
			Assert.That(module.ExposedModuleDependencies.Contains(origtestdependency), Is.True);
		}

		/// <summary>
		/// Tests loading the empty dependency list
		/// </summary>
		[Test]
		public void LoadNoDependencies()
		{
			//Setup
			var module = new TestEnhancedNinjectModule();
			var mockkernel = new Mock<IKernel>();
			module.OnLoad(mockkernel.Object);

			//Execute
			module.ExposedLoadDependencies();

			//Check
			mockkernel.Verify(k => k.Load(It.IsAny<List<NinjectModule>>()), Times.Never());
		}

		/// <summary>
		/// Tests loading the non-empty dependency list
		/// </summary>
		[Test]
		public void LoadDependencies()
		{
			//Setup
			var module = new TestEnhancedNinjectModule();
			var mockkernel = new Mock<IKernel>();
			module.OnLoad(mockkernel.Object);
			var testdependency = new TestDependency();
			mockkernel.Setup(k => k.HasModule(testdependency.GetType().FullName)).Returns(false);
			module.ExposedAddDependency(testdependency);

			//Execute
			module.ExposedLoadDependencies();

			//Check
			mockkernel.Verify(k => k.Load(module.ExposedModuleDependencies), Times.Once());
		}
	}
}
