# MandelbrotSIMD
A fast Mandelbrot fractal generator written in C# that utilizes the SIMD capabilities of the System.Numerics.Vectors library and multiprocessing.

This project was made in order to investigate the possibilities to use intrinsic SIMD functions of the CPU (SSE, AVX) in .NET. It is not possible to use intrinsic functions directly in managed code, but the *System.Numerics.Vectors* library provides a way to access these. 

A benefit of using this library is that you don't have to write code that handles specific levels of SIMD functions (the intrinsic functions are different for SSE and AVX). Instead, you get the number of values that can be calculated in a single instruction by reading the `Vector<T>.Count` property.

# Usage
Use the following commands to generate images:

<kbd>Q</kbd> Increase the scale by a factor of 2<br>
<kbd>A</kbd> Decrease the scale by a factor of 2<br>
<kbd>W</kbd> Increase the number of iterations by a factor of 2<br>
<kbd>S</kbd> Decrease the number of iterations by a factor of 2<br>

<kbd>Mouse-Left</kbd> Center the image at the location of the cursor<br>
<kbd>Mouse-Right</kbd> Center the image at the location of the cursor and increase the scale by a factor of 2

Each image that is generated is saved in a time-line and this time-line can be stepped through by <kbd>Left arrow</kbd> and <kbd>Right arrow</kbd>.
