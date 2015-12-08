# Adamant Temporary Compiler
The Adamant compiler is being bootstrapped.  To serve this process, a compiler must first be written in another language. In this case C#. This is that temporary compiler.  It will be thrown away when the compiler is re-written in Adamant.

## Project Status: Alpha Active
This is under active development.  It is in a very early stage and there are likely issues and limitations.  APIs are subject to frequent braking changes.

### Download and Use
Clone this git repo and compile using Visual Studio 2015.

## Explanation of this Project
The Adamant compiler is being bootstrapped.  To serve this process, a compiler must first be written in another language. In this case C#. This is that temporary compiler.  It will be thrown away when the compiler is re-written in Adamant.  To facilitate the re-writing in Adamant, this project is structured along lines that make sense for the Adamant compiler. C# was chosen for this because:

	* It is a high-level language
	* It supports many of the same language features as Adamant including:
		* Generics
		* Async Methods
		* Covariance and Contravariance (at least for interfaces)
	* C# being a garbage collected language means lifetimes will not have to be managed
	* I (Jeff Walker) am very familiar with C#

This approach has been adopted after trying to write an [Adamant to C# direct translator in C#](https://github.com/adamant/AdamantBootstrapCompiler).  That is a translator without type-checking, borrow checking or significant code transformations.  Indeed it didn't even build a symbol table.  However, this approach was found to be inadequate because there were important language features it just wasn't possible to translate this way (like covariant and contravariant types, type inference etc.).  After considering a number of alternatives, the current approach was settled on to minimize wasted worked and the time to get to the working front-end of an Adamant compiler.  That is a compiler capable of fully parsing and validating Adamant code, though not necessarily being able to compile into any target language/machine code.

## Architecting for re-write Guidelines
In order to facilitate the later re-write of the compiler into Adamant the following guidelines should be followed.

	* Code as if you were in the Adamant language.  For example, make use of the ability to pass `void` as a type parameter and async IO
	* Don't attempt to write most Adamant system libraries.	Rather, use the closest equivalent in the .NET framework.  This includes:
		* IO
		* Tasks (for async)
		* Character classes
	* Create classes and methods that can be translated to Adamant one-to-one
	* Interfaces are written as needed (rather than for every class as one might think is needed to represent Adamant)
	* Projects are named after the package they will be re-written to with the leading "Adamant." dropped from the named
	* Projects that would be under "System." in Adamant are named "Sys." instead to avoid conflicts with the C# standard library
	* The "Adamant.Core" project contains classes to bridge the gap between C# and Adamant language features.  For example, `Void` and `CodePoint`