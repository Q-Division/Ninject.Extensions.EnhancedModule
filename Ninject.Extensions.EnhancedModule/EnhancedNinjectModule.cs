//------------------------------------------------------------------------------
// <copyright file="EnhancedNinjectModule.cs" company="Q_Division">
//	Copyright (c) 2018 Q_Division. All rights reserved.
//
//  Licensed under MIT License (see LICENSE.txt for full license)
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Ninject.Modules;

namespace Ninject.Extensions.EnhancedModule
{
	/// <summary>
	/// Enhanced version of NinjectModule
	/// 
	/// Used when you might be loading dependencies that are already loaded by
	/// another module
	/// </summary>
	public abstract class EnhancedNinjectModule : NinjectModule
	{
		/// <summary>
		/// Internal list of dependencies. Used in the load call.
		/// </summary>
		protected List<NinjectModule> ModuleDependencies { get; }

		/// <summary>
		/// Constructor.
		/// 
		/// Creates the internal dependency list
		/// </summary>
		protected EnhancedNinjectModule()
		{
			ModuleDependencies = new List<NinjectModule>();
		}

		/// <summary>
		/// Adds a dependency. 
		/// Checks if one with the same name is already loaded; if one is it is
		/// skipped, if not it is added to the internal dependency list.
		/// </summary>
		/// <param name="dependency">Dependency to add</param>
		protected void AddDependency(NinjectModule dependency)
		{
			if (!Kernel.HasModule(dependency.GetType().FullName))
			{
				ModuleDependencies.Add(dependency);
			}
		}

		/// <summary>
		/// Load the dependecies in the internal list in the kernel.
		/// </summary>
		protected void LoadDependencies()
		{
			if (ModuleDependencies.Any())
			{
				Kernel.Load(ModuleDependencies);
			}
		}
	}
}
