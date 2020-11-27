# MandelbrotSIMD
A fast Mandelbrot fractal generator written in C# that utilizes the SIMD capabilities of the System.Numerics.Vectors library and multiprocessing.

# Usage
Use the following commands to generate images:

<kbd>Q</kbd> Increase the scale by a factor of 2<br>
<kbd>A</kbd> Decrease the scale by a factor of 2<br>
<kbd>W</kbd> Increase the number of iterations by a factor of 2<br>
<kbd>S</kbd> Decrease the number of iterations by a factor of 2<br>

<kbd>Mouse-Left</kbd> Center the image at the location of the cursor<br>
<kbd>Mouse-Right</kbd> Center the image at the location of the cursor and increase the scale by a factor of 2

Each image that is generated is saved in a time-line and this time-line can be stepped through by <kbd>Left arrow</kbd> and <kbd>Right arrow</kbd>.
