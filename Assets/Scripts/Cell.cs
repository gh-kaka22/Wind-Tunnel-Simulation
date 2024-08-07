using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public float density;
    public Vector3 velocity;
    // public Color color;
    public Material material;
    public int position;
}

/*
 * 
<!DOCTYPE HTML>
<!--
	A lattice-Boltzmann fluid simulation in JavaScript, using HTML5 canvas for graphics
	
	Copyright 2013, Daniel V. Schroeder

	Permission is hereby granted, free of charge, to any person obtaining a copy of 
	this software and associated data and documentation (the "Software"), to deal in 
	the Software without restriction, including without limitation the rights to 
	use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
	of the Software, and to permit persons to whom the Software is furnished to do 
	so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all 
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
	PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR 
	ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
	OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
	OTHER DEALINGS IN THE SOFTWARE.

	Except as contained in this notice, the name of the author shall not be used in 
	advertising or otherwise to promote the sale, use or other dealings in this 
	Software without prior written authorization.

	Credits:
	The "wind tunnel" entry/exit conditions are inspired by Graham Pullan's code
	(http://www.many-core.group.cam.ac.uk/projects/LBdemo.shtml).  Additional inspiration from 
	Thomas Pohl's applet (http://thomas-pohl.info/work/lba.html).  Other portions of code are based 
	on Wagner (http://www.ndsu.edu/physics/people/faculty/wagner/lattice_boltzmann_codes/) and
	Gonsalves (http://www.physics.buffalo.edu/phy411-506-2004/index.html; code adapted from Succi,
	http://global.oup.com/academic/product/the-lattice-boltzmann-equation-9780199679249).

	Revision history:
	First version, with only start/stop, speed, and viscosity controls, February 2013
	Added resolution control, mouse interaction, plot options, etc., March 2013
	Added sensor, contrast slider, improved tracer placement, Fy period readout, May 2013
	Added option to animate using setTimeout instead of requestAnimationFrame, July 2013
	Added "Flowline" plotting (actually just line segments), August 2013
	
	Still to do:
	* Fix the apparent bug in the force calculation that gives inconsistent results depending
		on initial conditions.  Perhaps bounce-backs between adjacent barrier sites don't cancel?
	* Grabbing the sensor while "drag fluid" selected causes a momentary drag at previous mouse location.
	* Try to pass two-fingered touch events on to the browser, so it's still possible to zoom in and out.
	* Work on GUI control layout, especially for smaller screens.
	* Treat ends symmetrically when flow speed is zero.
	* Try some other visualization techniques.
-->
<html>

<head>
<title>Fluid Dynamics Simulation</title>
<meta charset="utf-8">
<meta name="viewport" content="width=620">
<style>
	body {background-color:#ffffff; font-family:Arial, sans-serif; font-size:14px;
					text-align:center;}						/* gray background, center everything * /
	p {margin-left:auto; margin-right:auto; width:600px;}	/* keep paragraphs narrow and centered * /
	input {font-size:115%;}									/* make buttons bigger * /
	input[type="range"] {width:90px;}						/* make sliders shorter * /
	select {font-size:115%;}								/* make selectors bigger too * /
	li {text-align:left;}
</style>
</head>

<body>

<h2>Fluid Dynamics Simulation</h2>

<p>By <a href="http://physics.weber.edu/schroeder/">Dan Schroeder</a>, 
<a href="http://physics.weber.edu">Physics Department</a>, 
<a href="http://weber.edu">Weber State University</a></p>

<canvas id="theCanvas" width="600" height="240">This application runs only in modern
browsers. For best results, use Google Chrome.</canvas>

<div>
	<select id="sizeSelect" onchange="resize()">
		<option value="10">60 x 24</option>
		<option value="8">75 x 30</option>
		<option value="6">100 x 40</option>
		<option value="5">120 x 48</option>
		<option value="4">150 x 60</option>
		<option value="3">200 x 80</option>
		<option value="2">300 x 120</option>
		<option value="1">600 x 240</option>
	</select>
	<input id="resetFluidButton" type="button" onclick="initFluid()" value="Reset fluid">
	<input id="stepButton" type="button" onclick="simulate()" value="Step">
	<input id="startButton" type="button" onclick="startStop()" value="Start">
</div>
<div>
	Flow speed = <span id="speedValue">0.100</span> 
	<input id="speedSlider" type="range" min="0" max="0.12" step="0.005" value="0.1" onchange="adjustSpeed()">
	&nbsp;&nbsp;Viscosity = <span id="viscValue">0.020</span>
	<input id="viscSlider" type="range" min="0.005" max="0.2" step="0.005" value="0.02" onchange="adjustViscosity()">
</div>
<div style="margin-top:3px">
	<select id="mouseSelect">
		<option value="draw">Draw barriers</option>
		<option value="erase">Erase barriers</option>
		<option value="push">Drag fluid</option>
	</select>
	<select id="barrierSelect" onchange="placePresetBarrier()">
		<option>Barrier shapes</option>
	</select>
	<input id="clearButton" type="button" onclick="clearBarriers()" value="Clear barriers">
</div>
<div>
	<select id="plotSelect" onchange="paintCanvas()">
		<option>Plot density</option>
		<option>Plot x velocity</option>
		<option>Plot y velocity</option>
		<option>Plot speed</option>
		<option selected>Plot curl</option>
	</select>
	&nbsp;&nbsp;Contrast:
	<input id="contrastSlider" type="range" min="-10" max="10" step="1" value="0" onchange="paintCanvas()">
</div>
<div>
	Animation speed:
	<input id="stepsSlider" type="range" min="1" max="40" step="1" value="20" onchange="resetTimer()">
	&nbsp;&nbsp;Steps per second: <span id="speedReadout">0</span>
	&nbsp;&nbsp;<input id="rafCheck" type="checkbox" checked onchange="resetTimer()">Faster?
</div>
<div style="margin-top:4px">
	<!--<input id="pixelCheck" type="checkbox" checked onchange="resetTimer()">Use pixel graphics-->
	Show:
	<input id="tracerCheck" type="checkbox" onchange="initTracers()">Tracers
	<input id="flowlineCheck" type="checkbox" onchange="paintCanvas()">Flowlines
	<input id="forceCheck" type="checkbox" onchange="paintCanvas()">Force on barriers
	<input id="sensorCheck" type="checkbox" onchange="paintCanvas()">Sensor
	<input id="dataCheck" type="checkbox" onchange="showData()">Data
</div>
<div id="dataSection" style="display:none">
	<textarea id="dataArea" rows="8" cols="50" disabled readonly></textarea>
	<div>
		<input id="dataButton" type="button" value="Start data collection" onclick="startOrStopData()">
		<input id="periodButton" type="button" value="Show F_y period" onclick="showPeriod()">
		<input id="barrierDataButton" type="button" value="Show barrier locations" onclick="showBarrierLocations()">
		<input id="debugButton" type="button" value="Debug" onclick="debug()" style="display:none">
	</div>
</div>
<p style="text-align:left">This is a simulation of a two-dimensional fluid. Initially the fluid
is flowing from left to right, and a linear barrier (shown in black) diverts the fluid and creates 
vortices. The colors indicate the curl, or local rotational motion, of the fluid.  
Use the controls to adjust the flow speed and viscosity, draw different barriers, drag the
fluid around, plot other quantities besides the curl, show the force exerted by the fluid
on the barriers, and measure the fluid's density and velocity at any point. Enjoy!</p>

<p style="text-align:left">This simulation is intended for qualitative and semi-quantitative educational 
demonstrations&mdash;not for serious engineering use.  One obvious limitation is that it simulates
a fluid in only two dimensions rather than three.  It is also limited to modeling fluids at
constant temperature and with flow velocities that are at least a few times less than the speed of sound.
Perhaps the most important limitation, though, concerns the length and time scale.  The simulation
uses an arbitrary system of units, so the only way to compare to the real world is through the
dimensionless Reynolds number, defined as (length)(velocity)/(viscosity), where &ldquo;length&rdquo; 
is the characteristic size of whatever the fluid is flowing around or through.  The practical limit
on the Reynolds number in this simulation is a few hundred, whereas a typical Reynolds number for
air flowing around a bicyclist is roughly 100,000.  Higher Reynolds numbers result in more levels of
structure and turbulence in the fluid.  This simulation simply cannot handle the many high-Reynolds-number
situations that are so important in everyday life.
</p>

<p style="text-align:left">The simulation uses a fairly simple
<a href="http://en.wikipedia.org/wiki/Lattice_Boltzmann_methods">lattice-Boltzmann algorithm</a>, 
which you can see by viewing the JavaScript source code (use your browser's View Source or Page Source
menu command).  As of 2019, it runs at pleasing speeds on most personal computers in the Chrome, Firefox,
and Opera browsers.  Other browsers, not to mention mobile devices, may give inferior performance.
Some very old browsers may not even be able to display the slider controls.</p>

<p style="text-align:left">
	Related materials:
</p>
<div style="margin-left:auto; margin-right:auto; width:600px;">
	<ul>
	<li><a href="LatticeBoltzmannDemo.java.txt">A similar simulation in Java</a></li>
	<li><a href="LatticeBoltzmannDemo.py.txt">A similar simulation in Python</a></li>
	<li><a href="FluidSimulationsForUndergrads.pdf">Poster presentation</a> 
	given at the AAPT summer meeting, 2013 (pdf, 2.6 MB)</li>
	<li><a href="http://physics.weber.edu/schroeder/javacourse/LatticeBoltzmann.pdf">Instructions</a> 
	for a lattice-Boltzmann project in a computational physics course</li>
	<li>A more detailed explanation of the lattice-Boltzmann algorithm (coming soon)</li>
	</ul>
</div>

<script src="barrierdata.js"></script>
<script>
	// Global variables:	
	var mobile = navigator.userAgent.match(/iPhone|iPad|iPod|Android|BlackBerry|Opera Mini|IEMobile/i)
	var canvas = document.getElementById('theCanvas');
	var context = canvas.getContext('2d');
	var image = context.createImageData(canvas.width, canvas.height);		// for direct pixel manipulation (faster than fillRect)
	for (var i=3; i<image.data.length; i+=4) image.data[i] = 255;			// set all alpha values to opaque
	var sizeSelect = document.getElementById('sizeSelect');
	sizeSelect.selectedIndex = 5;
	if (mobile) sizeSelect.selectedIndex = 1;		// smaller works better on mobile platforms
	var pxPerSquare = Number(sizeSelect.options[sizeSelect.selectedIndex].value);
													// width of plotted grid site in pixels
	var xdim = canvas.width / pxPerSquare;			// grid dimensions for simulation
	var ydim = canvas.height / pxPerSquare;
	var stepsSlider = document.getElementById('stepsSlider');
	var startButton = document.getElementById('startButton');
	var speedSlider = document.getElementById('speedSlider');
	var speedValue = document.getElementById('speedValue');
	var viscSlider = document.getElementById('viscSlider');
	var viscValue = document.getElementById('viscValue');
	var mouseSelect = document.getElementById('mouseSelect');
	var barrierSelect = document.getElementById('barrierSelect');
	for (var barrierIndex=0; barrierIndex<barrierList.length; barrierIndex++) {
		var shape = document.createElement("option");
		shape.text = barrierList[barrierIndex].name;
		barrierSelect.add(shape, null);
	}
	var plotSelect = document.getElementById('plotSelect');
	var contrastSlider = document.getElementById('contrastSlider');
	//var pixelCheck = document.getElementById('pixelCheck');
	var tracerCheck = document.getElementById('tracerCheck');
	var flowlineCheck = document.getElementById('flowlineCheck');
	var forceCheck = document.getElementById('forceCheck');
	var sensorCheck = document.getElementById('sensorCheck');
	var dataCheck = document.getElementById('dataCheck');
	var rafCheck = document.getElementById('rafCheck');
	var speedReadout = document.getElementById('speedReadout');
	var dataSection = document.getElementById('dataSection');
	var dataArea = document.getElementById('dataArea');
	var dataButton = document.getElementById('dataButton');
	var running = false;						// will be true when running
	var stepCount = 0;
	var startTime = 0;
	var four9ths = 4.0 / 9.0;					// abbreviations
	var one9th = 1.0 / 9.0;
	var one36th = 1.0 / 36.0;
	var barrierCount = 0;
	var barrierxSum = 0;
	var barrierySum = 0;
	var barrierFx = 0.0;						// total force on all barrier sites
	var barrierFy = 0.0;
	var sensorX = xdim / 2;						// coordinates of "sensor" to measure local fluid properties	
	var sensorY = ydim / 2;
	var draggingSensor = false;
	var mouseIsDown = false;
	var mouseX, mouseY;							// mouse location in canvas coordinates
	var oldMouseX = -1, oldMouseY = -1;			// mouse coordinates from previous simulation frame
	var collectingData = false;
	var time = 0;								// time (in simulation step units) since data collection started
	var showingPeriod = false;
	var lastBarrierFy = 1;						// for determining when F_y oscillation begins
	var lastFyOscTime = 0;						// for calculating F_y oscillation period

	canvas.addEventListener('mousedown', mouseDown, false);
	canvas.addEventListener('mousemove', mouseMove, false);
	document.body.addEventListener('mouseup', mouseUp, false);	// button release could occur outside canvas
	canvas.addEventListener('touchstart', mouseDown, false);
	canvas.addEventListener('touchmove', mouseMove, false);
	document.body.addEventListener('touchend', mouseUp, false);

	// Create the arrays of fluid particle densities, etc. (using 1D arrays for speed):
	// To index into these arrays, use x + y*xdim, traversing rows first and then columns.
	var n0 = new Array(xdim*ydim);			// microscopic densities along each lattice direction
	var nN = new Array(xdim*ydim);
	var nS = new Array(xdim*ydim);
	var nE = new Array(xdim*ydim);
	var nW = new Array(xdim*ydim);
	var nNE = new Array(xdim*ydim);
	var nSE = new Array(xdim*ydim);
	var nNW = new Array(xdim*ydim);
	var nSW = new Array(xdim*ydim);
	var rho = new Array(xdim*ydim);			// macroscopic density
	var ux = new Array(xdim*ydim);			// macroscopic velocity
	var ux = new Array(xdim*ydim);			// macroscopic velocity
	var uy = new Array(xdim*ydim);
	var curl = new Array(xdim*ydim);
	var barrier = new Array(xdim*ydim);		// boolean array of barrier locations

	// Initialize to a steady rightward flow with no barriers:
	for (var y=0; y<ydim; y++) {
		for (var x=0; x<xdim; x++) {
			barrier[x+y*xdim] = false;
		}
	}

	// Create a simple linear "wall" barrier (intentionally a little offset from center):
	var barrierSize = 8;
	if (mobile) barrierSize = 4;
	for (var y=(ydim/2)-barrierSize; y<=(ydim/2)+barrierSize; y++) {
		var x = Math.round(ydim/3);
		barrier[x+y*xdim] = true;
	}

	// Set up the array of colors for plotting (mimicks matplotlib "jet" colormap):
	// (Kludge: Index nColors+1 labels the color used for drawing barriers.)
	var nColors = 400;							// there are actually nColors+2 colors
	var hexColorList = new Array(nColors+2);
	var redList = new Array(nColors+2);
	var greenList = new Array(nColors+2);
	var blueList = new Array(nColors+2);
	for (var c=0; c<=nColors; c++) {
		var r, g, b;
		if (c < nColors/8) {
			r = 0; g = 0; b = Math.round(255 * (c + nColors/8) / (nColors/4));
		} else if (c < 3*nColors/8) {
			r = 0; g = Math.round(255 * (c - nColors/8) / (nColors/4)); b = 255;
		} else if (c < 5*nColors/8) {
			r = Math.round(255 * (c - 3*nColors/8) / (nColors/4)); g = 255; b = 255 - r;
		} else if (c < 7*nColors/8) {
			r = 255; g = Math.round(255 * (7*nColors/8 - c) / (nColors/4)); b = 0;
		} else {
			r = Math.round(255 * (9*nColors/8 - c) / (nColors/4)); g = 0; b = 0;
		}
		redList[c] = r; greenList[c] = g; blueList[c] = b;
		hexColorList[c] = rgbToHex(r, g, b);
	}
	redList[nColors+1] = 0; greenList[nColors+1] = 0; blueList[nColors+1] = 0;	// barriers are black
	hexColorList[nColors+1] = rgbToHex(0, 0, 0);

	// Functions to convert rgb to hex color string (from stackoverflow):
	function componentToHex(c) {
		var hex = c.toString(16);
		return hex.length == 1 ? "0" + hex : hex;
	}
	function rgbToHex(r, g, b) {
		return "#" + componentToHex(r) + componentToHex(g) + componentToHex(b);
	}

	// Initialize array of partially transparant blacks, for drawing flow lines:
	var transBlackArraySize = 50;
	var transBlackArray = new Array(transBlackArraySize);
	for (var i=0; i<transBlackArraySize; i++) {
		transBlackArray[i] = "rgba(0,0,0," + Number(i/transBlackArraySize).toFixed(2) + ")";
	}

	// Initialize tracers (but don't place them yet):
	var nTracers = 144;
	var tracerX = new Array(nTracers);
	var tracerY = new Array(nTracers);
	for (var t=0; t<nTracers; t++) {
		tracerX[t] = 0.0; tracerY[t] = 0.0;
	}

	initFluid();		// initialize to steady rightward flow

	// Mysterious gymnastics that are apparently useful for better cross-browser animation timing:
	window.requestAnimFrame = (function(callback) {
		return 	window.requestAnimationFrame || 
			window.webkitRequestAnimationFrame || 
			window.mozRequestAnimationFrame || 
			window.oRequestAnimationFrame || 
			window.msRequestAnimationFrame ||
			function(callback) {
				window.setTimeout(callback, 1);		// second parameter is time in ms
			};
	})();

	// Simulate function executes a bunch of steps and then schedules another call to itself:
	function simulate() {
		var stepsPerFrame = Number(stepsSlider.value);			// number of simulation steps per animation frame
		setBoundaries();
		// Test to see if we're dragging the fluid:
		var pushing = false;
		var pushX, pushY, pushUX, pushUY;
		if (mouseIsDown && mouseSelect.selectedIndex==2) {
			if (oldMouseX >= 0) {
				var gridLoc = canvasToGrid(mouseX, mouseY);
				pushX = gridLoc.x;
				pushY = gridLoc.y;
				pushUX = (mouseX - oldMouseX) / pxPerSquare / stepsPerFrame;
				pushUY = -(mouseY - oldMouseY) / pxPerSquare / stepsPerFrame;	// y axis is flipped
				if (Math.abs(pushUX) > 0.1) pushUX = 0.1 * Math.abs(pushUX) / pushUX;
				if (Math.abs(pushUY) > 0.1) pushUY = 0.1 * Math.abs(pushUY) / pushUY;
				pushing = true;
			}
			oldMouseX = mouseX; oldMouseY = mouseY;
		} else {
			oldMouseX = -1; oldMouseY = -1;
		}
		// Execute a bunch of time steps:
		for (var step=0; step<stepsPerFrame; step++) {
			collide();
			stream();
			if (tracerCheck.checked) moveTracers();
			if (pushing) push(pushX, pushY, pushUX, pushUY);
			time++;
			if (showingPeriod && (barrierFy > 0) && (lastBarrierFy <=0)) {
				var thisFyOscTime = time - barrierFy/(barrierFy-lastBarrierFy);	// interpolate when Fy changed sign
				if (lastFyOscTime > 0) {
					var period = thisFyOscTime - lastFyOscTime;
					dataArea.innerHTML += Number(period).toFixed(2) + "\n";
					dataArea.scrollTop = dataArea.scrollHeight;
				}
				lastFyOscTime = thisFyOscTime;
			}
			lastBarrierFy = barrierFy;
		}
		paintCanvas();
		if (collectingData) {
			writeData();
			if (time >= 10000) startOrStopData();
		}
		if (running) {
			stepCount += stepsPerFrame;
			var elapsedTime = ((new Date()).getTime() - startTime) / 1000;	// time in seconds
			speedReadout.innerHTML = Number(stepCount/elapsedTime).toFixed(0);
		}
		var stable = true;
		for (var x=0; x<xdim; x++) {
			var index = x + (ydim/2)*xdim;	// look at middle row only
			if (rho[index] <= 0) stable = false;
		}
		if (!stable) {
			window.alert("The simulation has become unstable due to excessive fluid speeds.");
			startStop();
			initFluid();
		}
		if (running) {
			if (rafCheck.checked) {
				requestAnimFrame(function() { simulate(); });	// let browser schedule next frame
			} else {
				window.setTimeout(simulate, 1);	// schedule next frame asap (nominally 1 ms but always more)
			}
		}
	}

	// Set the fluid variables at the boundaries, according to the current slider value:
	function setBoundaries() {
		var u0 = Number(speedSlider.value);
		for (var x=0; x<xdim; x++) {
			setEquil(x, 0, u0, 0, 1);
			setEquil(x, ydim-1, u0, 0, 1);
		}
		for (var y=1; y<ydim-1; y++) {
			setEquil(0, y, u0, 0, 1);
			setEquil(xdim-1, y, u0, 0, 1);
		}
	}

	// Collide particles within each cell (here's the physics!):
	function collide() {
		var viscosity = Number(viscSlider.value);	// kinematic viscosity coefficient in natural units
		var omega = 1 / (3*viscosity + 0.5);		// reciprocal of relaxation time
		for (var y=1; y<ydim-1; y++) {
			for (var x=1; x<xdim-1; x++) {
				var i = x + y*xdim;		// array index for this lattice site
				var thisrho = n0[i] + nN[i] + nS[i] + nE[i] + nW[i] + nNW[i] + nNE[i] + nSW[i] + nSE[i];
				rho[i] = thisrho;
				var thisux = (nE[i] + nNE[i] + nSE[i] - nW[i] - nNW[i] - nSW[i]) / thisrho;
				ux[i] = thisux;
				var thisuy = (nN[i] + nNE[i] + nNW[i] - nS[i] - nSE[i] - nSW[i]) / thisrho;
				uy[i] = thisuy
				var one9thrho = one9th * thisrho;		// pre-compute a bunch of stuff for optimization
				var one36thrho = one36th * thisrho;
				var ux3 = 3 * thisux;
				var uy3 = 3 * thisuy;
				var ux2 = thisux * thisux;
				var uy2 = thisuy * thisuy;
				var uxuy2 = 2 * thisux * thisuy;
				var u2 = ux2 + uy2;
				var u215 = 1.5 * u2;
				n0[i]  += omega * (four9ths*thisrho * (1                          - u215) - n0[i]);
				nE[i]  += omega * (   one9thrho * (1 + ux3       + 4.5*ux2        - u215) - nE[i]);
				nW[i]  += omega * (   one9thrho * (1 - ux3       + 4.5*ux2        - u215) - nW[i]);
				nN[i]  += omega * (   one9thrho * (1 + uy3       + 4.5*uy2        - u215) - nN[i]);
				nS[i]  += omega * (   one9thrho * (1 - uy3       + 4.5*uy2        - u215) - nS[i]);
				nNE[i] += omega * (  one36thrho * (1 + ux3 + uy3 + 4.5*(u2+uxuy2) - u215) - nNE[i]);
				nSE[i] += omega * (  one36thrho * (1 + ux3 - uy3 + 4.5*(u2-uxuy2) - u215) - nSE[i]);
				nNW[i] += omega * (  one36thrho * (1 - ux3 + uy3 + 4.5*(u2-uxuy2) - u215) - nNW[i]);
				nSW[i] += omega * (  one36thrho * (1 - ux3 - uy3 + 4.5*(u2+uxuy2) - u215) - nSW[i]);
			}
		}
		for (var y=1; y<ydim-2; y++) {
			nW[xdim-1+y*xdim] = nW[xdim-2+y*xdim];		// at right end, copy left-flowing densities from next row to the left
			nNW[xdim-1+y*xdim] = nNW[xdim-2+y*xdim];
			nSW[xdim-1+y*xdim] = nSW[xdim-2+y*xdim];
		}
	}

	// Move particles along their directions of motion:
	function stream() {
		barrierCount = 0; barrierxSum = 0; barrierySum = 0;
		barrierFx = 0.0; barrierFy = 0.0;
		for (var y=ydim-2; y>0; y--) {			// first start in NW corner...
			for (var x=1; x<xdim-1; x++) {
				nN[x+y*xdim] = nN[x+(y-1)*xdim];			// move the north-moving particles
				nNW[x+y*xdim] = nNW[x+1+(y-1)*xdim];		// and the northwest-moving particles
			}
		}
		for (var y=ydim-2; y>0; y--) {			// now start in NE corner...
			for (var x=xdim-2; x>0; x--) {
				nE[x+y*xdim] = nE[x-1+y*xdim];			// move the east-moving particles
				nNE[x+y*xdim] = nNE[x-1+(y-1)*xdim];		// and the northeast-moving particles
			}
		}
		for (var y=1; y<ydim-1; y++) {			// now start in SE corner...
			for (var x=xdim-2; x>0; x--) {
				nS[x+y*xdim] = nS[x+(y+1)*xdim];			// move the south-moving particles
				nSE[x+y*xdim] = nSE[x-1+(y+1)*xdim];		// and the southeast-moving particles
			}
		}
		for (var y=1; y<ydim-1; y++) {				// now start in the SW corner...
			for (var x=1; x<xdim-1; x++) {
				nW[x+y*xdim] = nW[x+1+y*xdim];			// move the west-moving particles
				nSW[x+y*xdim] = nSW[x+1+(y+1)*xdim];		// and the southwest-moving particles
			}
		}
		for (var y=1; y<ydim-1; y++) {				// Now handle bounce-back from barriers
			for (var x=1; x<xdim-1; x++) {
				if (barrier[x+y*xdim]) {
					var index = x + y*xdim;
					nE[x+1+y*xdim] = nW[index];
					nW[x-1+y*xdim] = nE[index];
					nN[x+(y+1)*xdim] = nS[index];
					nS[x+(y-1)*xdim] = nN[index];
					nNE[x+1+(y+1)*xdim] = nSW[index];
					nNW[x-1+(y+1)*xdim] = nSE[index];
					nSE[x+1+(y-1)*xdim] = nNW[index];
					nSW[x-1+(y-1)*xdim] = nNE[index];
					// Keep track of stuff needed to plot force vector:
					barrierCount++;
					barrierxSum += x;
					barrierySum += y;
					barrierFx += nE[index] + nNE[index] + nSE[index] - nW[index] - nNW[index] - nSW[index];
					barrierFy += nN[index] + nNE[index] + nNW[index] - nS[index] - nSE[index] - nSW[index];
				}
			}
		}
	}

	// Move the tracer particles:
	function moveTracers() {
		for (var t=0; t<nTracers; t++) {
			var roundedX = Math.round(tracerX[t]);
			var roundedY = Math.round(tracerY[t]);
			var index = roundedX + roundedY*xdim;
			tracerX[t] += ux[index];
			tracerY[t] += uy[index];
			if (tracerX[t] > xdim-1) {
				tracerX[t] = 0;
				tracerY[t] = Math.random() * ydim;
			}
		}
	}

	// "Drag" the fluid in a direction determined by the mouse (or touch) motion:
	// (The drag affects a "circle", 5 px in diameter, centered on the given coordinates.)
	function push(pushX, pushY, pushUX, pushUY) {
		// First make sure we're not too close to edge:
		var margin = 3;
		if ((pushX > margin) && (pushX < xdim-1-margin) && (pushY > margin) && (pushY < ydim-1-margin)) {
			for (var dx=-1; dx<=1; dx++) {
				setEquil(pushX+dx, pushY+2, pushUX, pushUY);
				setEquil(pushX+dx, pushY-2, pushUX, pushUY);
			}
			for (var dx=-2; dx<=2; dx++) {
				for (var dy=-1; dy<=1; dy++) {
					setEquil(pushX+dx, pushY+dy, pushUX, pushUY);
				}
			}
		}
	}

	// Set all densities in a cell to their equilibrium values for a given velocity and density:
	// (If density is omitted, it's left unchanged.)
	function setEquil(x, y, newux, newuy, newrho) {
		var i = x + y*xdim;
		if (typeof newrho == 'undefined') {
			newrho = rho[i];
		}
		var ux3 = 3 * newux;
		var uy3 = 3 * newuy;
		var ux2 = newux * newux;
		var uy2 = newuy * newuy;
		var uxuy2 = 2 * newux * newuy;
		var u2 = ux2 + uy2;
		var u215 = 1.5 * u2;
		n0[i]  = four9ths * newrho * (1                              - u215);
		nE[i]  =   one18th * newrho * (1 + ux3       + 4.5*ux2        - u215);
		nW[i]  =   one18th * newrho * (1 - ux3       + 4.5*ux2        - u215);
		nN[i]  =   one18th * newrho * (1 + uy3       + 4.5*uy2        - u215);
		nS[i]  =   one18th * newrho * (1 - uy3       + 4.5*uy2        - u215);
		nF[i]  =   one18th * newrho * (1 + uz3       + 4.5*uz2        - u215);//
		nB[i]  =   one18th * newrho * (1 - uz3       + 4.5*uz2        - u215);//
		nNE[i] =  one36th * newrho * (1 + ux3 + uy3 + 4.5*(u2+uxuy2) - u215);
		nSE[i] =  one36th * newrho * (1 + ux3 - uy3 + 4.5*(u2-uxuy2) - u215);
		nNW[i] =  one36th * newrho * (1 - ux3 + uy3 + 4.5*(u2-uxuy2) - u215);
		nSW[i] =  one36th * newrho * (1 - ux3 - uy3 + 4.5*(u2+uxuy2) - u215);
		nFE[i] =  one36th * newrho * (1 + ux3 + uz3 + 4.5*(u2+uxuz2) - u215);
		nFW[i] =  one36th * newrho * (1 - ux3 + uz3 + 4.5*(u2-uxuz2) - u215);
		nFN[i] =  one36th * newrho * (1 + uy3 + uz3 + 4.5*(u2+uyuz2) - u215);
		nFS[i] =  one36th * newrho * (1 - uy3 + uz3 + 4.5*(u2-uyuz2) - u215);
		nBE[i] =  one36th * newrho * (1 + ux3 - uz3 + 4.5*(u2-uxuz2) - u215);
		nBW[i] =  one36th * newrho * (1 - ux3 - uz3 + 4.5*(u2+uxuz2) - u215);
		nBN[i] =  one36th * newrho * (1 + uy3 - uz3 + 4.5*(u2-uyuz2) - u215);
		nBS[i] =  one36th * newrho * (1 - uy3 - uz3 + 4.5*(u2+uyuz2) - u215);
		
		rho[i] = newrho;
		ux[i] = newux;
		uy[i] = newuy;
	}

	// Initialize the tracer particles:
	function initTracers() {
		if (tracerCheck.checked) {
			var nRows = Math.ceil(Math.sqrt(nTracers));
			var dx = xdim / nRows;
			var dy = ydim / nRows;
			var nextX = dx / 2;
			var nextY = dy / 2;
			for (var t=0; t<nTracers; t++) {
				tracerX[t] = nextX;
				tracerY[t] = nextY;
				nextX += dx;
				if (nextX > xdim) {
					nextX = dx / 2;
					nextY += dy;
				}
			}
		}
		paintCanvas();
	}

	// Paint the canvas:
	function paintCanvas() {    
		var cIndex=0;
		var contrast = Math.pow(1.2,Number(contrastSlider.value));
		var plotType = plotSelect.selectedIndex;
		//var pixelGraphics = pixelCheck.checked;
		if (plotType == 4) computeCurl();
		for (var y=0; y<ydim; y++) {
			for (var x=0; x<xdim; x++) {
				if (barrier[x+y*xdim]) {
					cIndex = nColors + 1;	// kludge for barrier color which isn't really part of color map
				} else {
					if (plotType == 0) {
						cIndex = Math.round(nColors * ((rho[x+y*xdim]-1)*6*contrast + 0.5));
					} else if (plotType == 1) {
						cIndex = Math.round(nColors * (ux[x+y*xdim]*2*contrast + 0.5));
					} else if (plotType == 2) {
						cIndex = Math.round(nColors * (uy[x+y*xdim]*2*contrast + 0.5));
					} else if (plotType == 3) {
						var speed = Math.sqrt(ux[x+y*xdim]*ux[x+y*xdim] + uy[x+y*xdim]*uy[x+y*xdim]);
						cIndex = Math.round(nColors * (speed*4*contrast));
					} else {
						cIndex = Math.round(nColors * (curl[x+y*xdim]*5*contrast + 0.5));
					}
					if (cIndex < 0) cIndex = 0;
					if (cIndex > nColors) cIndex = nColors;
				}
				//if (pixelGraphics) {
					//colorSquare(x, y, cIndex);
				colorSquare(x, y, redList[cIndex], greenList[cIndex], blueList[cIndex]);
				//} else {
				//	context.fillStyle = hexColorList[cIndex];
				//	context.fillRect(x*pxPerSquare, (ydim-y-1)*pxPerSquare, pxPerSquare, pxPerSquare);
				//}
			}
		}
		//if (pixelGraphics) 
		context.putImageData(image, 0, 0);		// blast image to the screen
		// Draw tracers, force vector, and/or sensor if appropriate:
		if (tracerCheck.checked) drawTracers();
		if (flowlineCheck.checked) drawFlowlines();
		if (forceCheck.checked) drawForceArrow(barrierxSum/barrierCount, barrierySum/barrierCount, barrierFx, barrierFy);
		if (sensorCheck.checked) drawSensor();
	}

	// Color a grid square in the image data array, one pixel at a time (rgb each in range 0 to 255):
	function colorSquare(x, y, r, g, b) {
	//function colorSquare(x, y, cIndex) {		// for some strange reason, this version is quite a bit slower on Chrome
		//var r = redList[cIndex];
		//var g = greenList[cIndex];
		//var b = blueList[cIndex];
		var flippedy = ydim - y - 1;			// put y=0 at the bottom
		for (var py=flippedy*pxPerSquare; py<(flippedy+1)*pxPerSquare; py++) {
			for (var px=x*pxPerSquare; px<(x+1)*pxPerSquare; px++) {
				var index = (px + py*image.width) * 4;
				image.data[index+0] = r;
				image.data[index+1] = g;
				image.data[index+2] = b;
			}
		}
	}

	// Compute the curl (actually times 2) of the macroscopic velocity field, for plotting:
	function computeCurl() {
		for (var y=1; y<ydim-1; y++) {			// interior sites only; leave edges set to zero
			for (var x=1; x<xdim-1; x++) {
				curl[x+y*xdim] = uy[x+1+y*xdim] - uy[x-1+y*xdim] - ux[x+(y+1)*xdim] + ux[x+(y-1)*xdim];
			}
		}
	}

	// Draw the tracer particles:
	function drawTracers() {
		context.fillStyle = "rgb(150,150,150)";
		for (var t=0; t<nTracers; t++) {
			var canvasX = (tracerX[t]+0.5) * pxPerSquare;
			var canvasY = canvas.height - (tracerY[t]+0.5) * pxPerSquare;
			context.fillRect(canvasX-1, canvasY-1, 2, 2);
		}
	}

	// Draw a grid of short line segments along flow directions:
	function drawFlowlines() {
		var pxPerFlowline = 10;
		if (pxPerSquare == 1) pxPerFlowline = 6;
		if (pxPerSquare == 2) pxPerFlowline = 8;
		if (pxPerSquare == 5) pxPerFlowline = 12;
		if ((pxPerSquare == 6) || (pxPerSquare == 8)) pxPerFlowline = 15;
		if (pxPerSquare == 10) pxPerFlowline = 20;
		var sitesPerFlowline = pxPerFlowline / pxPerSquare;
		var xLines = canvas.width / pxPerFlowline;
		var yLines = canvas.height / pxPerFlowline;
		for (var yCount=0; yCount<yLines; yCount++) {
			for (var xCount=0; xCount<xLines; xCount++) {
				var x = Math.round((xCount+0.5) * sitesPerFlowline);
				var y = Math.round((yCount+0.5) * sitesPerFlowline);
				var thisUx = ux[x+y*xdim];
				var thisUy = uy[x+y*xdim];
				var speed = Math.sqrt(thisUx*thisUx + thisUy*thisUy);
				if (speed > 0.0001) {
					var px = (xCount+0.5) * pxPerFlowline;
					var py = canvas.height - ((yCount+0.5) * pxPerFlowline);
					var scale = 0.5 * pxPerFlowline / speed;
					context.beginPath();
					context.moveTo(px-thisUx*scale, py+thisUy*scale);
					context.lineTo(px+thisUx*scale, py-thisUy*scale);
					//context.lineWidth = speed * 5;
					var cIndex = Math.round(speed * transBlackArraySize / 0.3);
					if (cIndex >= transBlackArraySize) cIndex = transBlackArraySize - 1;
					context.strokeStyle = transBlackArray[cIndex];
					//context.strokeStyle = "rgba(0,0,0,0.1)";
					context.stroke();
				}
			}
		}
	}

	// Draw an arrow to represent the total force on the barrier(s):
	function drawForceArrow(x, y, Fx, Fy) {
		context.fillStyle = "rgba(100,100,100,0.7)";
		context.translate((x + 0.5) * pxPerSquare, canvas.height - (y + 0.5) * pxPerSquare);
		var magF = Math.sqrt(Fx*Fx + Fy*Fy);
		context.scale(4*magF, 4*magF);
		context.rotate(Math.atan2(-Fy, Fx));
		context.beginPath();
		context.moveTo(0, 3);
		context.lineTo(100, 3);
		context.lineTo(100, 12);
		context.lineTo(130, 0);
		context.lineTo(100, -12);
		context.lineTo(100, -3);
		context.lineTo(0, -3);
		context.lineTo(0, 3);
		context.fill();
		context.setTransform(1, 0, 0, 1, 0, 0);
	}

	// Draw the sensor and its associated data display:
	function drawSensor() {
		var canvasX = (sensorX+0.5) * pxPerSquare;
		var canvasY = canvas.height - (sensorY+0.5) * pxPerSquare;
		context.fillStyle = "rgba(180,180,180,0.7)";	// first draw gray filled circle
		context.beginPath();
		context.arc(canvasX, canvasY, 7, 0, 2*Math.PI);
		context.fill();
		context.strokeStyle = "#404040";				// next draw cross-hairs
		context.linewidth = 1;
		context.beginPath();
		context.moveTo(canvasX, canvasY-10);
		context.lineTo(canvasX, canvasY+10);
		context.moveTo(canvasX-10, canvasY);
		context.lineTo(canvasX+10, canvasY);
		context.stroke();
		context.fillStyle = "rgba(255,255,255,0.5)";	// draw rectangle behind text
		canvasX += 10;
		context.font = "12px Monospace";
		var rectWidth = context.measureText("00000000000").width+6;
		var rectHeight = 58;
		if (canvasX+rectWidth > canvas.width) canvasX -= (rectWidth+20);
		if (canvasY+rectHeight > canvas.height) canvasY = canvas.height - rectHeight;
		context.fillRect(canvasX, canvasY, rectWidth, rectHeight);
		context.fillStyle = "#000000";					// finally draw the text
		canvasX += 3;
		canvasY += 12;
		var coordinates = "  (" + sensorX + "," + sensorY + ")";
		context.fillText(coordinates, canvasX, canvasY);
		canvasY += 14;
		var rhoSymbol = String.fromCharCode(parseInt('03C1',16));
		var index = sensorX + sensorY * xdim;
		context.fillText(" " + rhoSymbol + " =  " + Number(rho[index]).toFixed(3), canvasX, canvasY);
		canvasY += 14;
		var digitString = Number(ux[index]).toFixed(3);
		if (ux[index] >= 0) digitString = " " + digitString;
		context.fillText("ux = " + digitString, canvasX, canvasY);
		canvasY += 14;
		digitString = Number(uy[index]).toFixed(3);
		if (uy[index] >= 0) digitString = " " + digitString;
		context.fillText("uy = " + digitString, canvasX, canvasY);
	}

	// Functions to handle mouse/touch interaction:
	function mouseDown(e) {
		if (sensorCheck.checked) {
			var canvasLoc = pageToCanvas(e.pageX, e.pageY);
			var gridLoc = canvasToGrid(canvasLoc.x, canvasLoc.y);
			var dx = (gridLoc.x - sensorX) * pxPerSquare;
			var dy = (gridLoc.y - sensorY) * pxPerSquare;
			if (Math.sqrt(dx*dx + dy*dy) <= 8) {
				draggingSensor = true;
			}
		}
		mousePressDrag(e);
	};
	function mouseMove(e) {
		if (mouseIsDown) {
			mousePressDrag(e);
		}
	};
	function mouseUp(e) {
		mouseIsDown = false;
		draggingSensor = false;
	};

	// Handle mouse press or drag:
	function mousePressDrag(e) {
		e.preventDefault();
		mouseIsDown = true;
		var canvasLoc = pageToCanvas(e.pageX, e.pageY);
		if (draggingSensor) {
			var gridLoc = canvasToGrid(canvasLoc.x, canvasLoc.y);
			sensorX = gridLoc.x;
			sensorY = gridLoc.y;
			paintCanvas();
			return;
		}
		if (mouseSelect.selectedIndex == 2) {
			mouseX = canvasLoc.x;
			mouseY = canvasLoc.y;
			return;
		}
		var gridLoc = canvasToGrid(canvasLoc.x, canvasLoc.y);
		if (mouseSelect.selectedIndex == 0) {
			addBarrier(gridLoc.x, gridLoc.y);
			paintCanvas();
		} else {
			removeBarrier(gridLoc.x, gridLoc.y);
		}
	}

	// Convert page coordinates to canvas coordinates:
	function pageToCanvas(pageX, pageY) {
		var canvasX = pageX - canvas.offsetLeft;
		var canvasY = pageY - canvas.offsetTop;
		// this simple subtraction may not work when the canvas is nested in other elements
		return { x:canvasX, y:canvasY };
	}

	// Convert canvas coordinates to grid coordinates:
	function canvasToGrid(canvasX, canvasY) {
		var gridX = Math.floor(canvasX / pxPerSquare);
		var gridY = Math.floor((canvas.height - 1 - canvasY) / pxPerSquare); 	// off by 1?
		return { x:gridX, y:gridY };
	}

	// Add a barrier at a given grid coordinate location:
	function addBarrier(x, y) {
		if ((x > 1) && (x < xdim-2) && (y > 1) && (y < ydim-2)) {
			barrier[x+y*xdim] = true;
		}
	}

	// Remove a barrier at a given grid coordinate location:
	function removeBarrier(x, y) {
		if (barrier[x+y*xdim]) {
			barrier[x+y*xdim] = false;
			paintCanvas();
		}
	}

	// Clear all barriers:
	function clearBarriers() {
		for (var x=0; x<xdim; x++) {
			for (var y=0; y<ydim; y++) {
				barrier[x+y*xdim] = false;
			}
		}
		paintCanvas();
	}

	// Resize the grid:
	function resize() {
		// First up-sample the macroscopic variables into temporary arrays at max resolution:
		var tempRho = new Array(canvas.width*canvas.height);
		var tempUx = new Array(canvas.width*canvas.height);
		var tempUy = new Array(canvas.width*canvas.height);
		var tempBarrier = new Array(canvas.width*canvas.height);
		for (var y=0; y<canvas.height; y++) {
			for (var x=0; x<canvas.width; x++) {
				var tempIndex = x + y*canvas.width;
				var xOld = Math.floor(x / pxPerSquare);
				var yOld = Math.floor(y / pxPerSquare);
				var oldIndex = xOld + yOld*xdim;
				tempRho[tempIndex] = rho[oldIndex];
				tempUx[tempIndex] = ux[oldIndex];
				tempUy[tempIndex] = uy[oldIndex];
				tempBarrier[tempIndex] = barrier[oldIndex];
			}
		}
		// Get new size from GUI selector:
		var oldPxPerSquare = pxPerSquare;
		pxPerSquare = Number(sizeSelect.options[sizeSelect.selectedIndex].value);
		var growRatio = oldPxPerSquare / pxPerSquare;
		xdim = canvas.width / pxPerSquare;
		ydim = canvas.height / pxPerSquare;
		// Create new arrays at the desired resolution:
		n0 = new Array(xdim*ydim);
		nN = new Array(xdim*ydim);
		nS = new Array(xdim*ydim);
		nE = new Array(xdim*ydim);
		nW = new Array(xdim*ydim);
		nNE = new Array(xdim*ydim);
		nSE = new Array(xdim*ydim);
		nNW = new Array(xdim*ydim);
		nSW = new Array(xdim*ydim);
		rho = new Array(xdim*ydim);
		ux = new Array(xdim*ydim);
		uy = new Array(xdim*ydim);
		curl = new Array(xdim*ydim);
		barrier = new Array(xdim*ydim);
		// Down-sample the temporary arrays into the new arrays:
		for (var yNew=0; yNew<ydim; yNew++) {
			for (var xNew=0; xNew<xdim; xNew++) {
				var rhoTotal = 0;
				var uxTotal = 0;
				var uyTotal = 0;
				var barrierTotal = 0;
				for (var y=yNew*pxPerSquare; y<(yNew+1)*pxPerSquare; y++) {
					for (var x=xNew*pxPerSquare; x<(xNew+1)*pxPerSquare; x++) {
						var index = x + y*canvas.width;
						rhoTotal += tempRho[index];
						uxTotal += tempUx[index];
						uyTotal += tempUy[index];
						if (tempBarrier[index]) barrierTotal++;
					}
				}
				setEquil(xNew, yNew, uxTotal/(pxPerSquare*pxPerSquare), uyTotal/(pxPerSquare*pxPerSquare), rhoTotal/(pxPerSquare*pxPerSquare))
				curl[xNew+yNew*xdim] = 0.0;
				barrier[xNew+yNew*xdim] = (barrierTotal >= pxPerSquare*pxPerSquare/2);
			}
		}
		setBoundaries();
		if (tracerCheck.checked) {
			for (var t=0; t<nTracers; t++) {
				tracerX[t] *= growRatio;
				tracerY[t] *= growRatio;
			}
		}
		sensorX = Math.round(sensorX * growRatio);
		sensorY = Math.round(sensorY * growRatio);
		//computeCurl();
		paintCanvas();
		resetTimer();
	}

	// Function to initialize or re-initialize the fluid, based on speed slider setting:
	function initFluid() {
		// Amazingly, if I nest the y loop inside the x loop, Firefox slows down by a factor of 20
		var u0 = Number(speedSlider.value);
		for (var y=0; y<ydim; y++) {
			for (var x=0; x<xdim; x++) {
				setEquil(x, y, u0, 0, 1);
				curl[x+y*xdim] = 0.0;
			}
		}
	paintCanvas();
	}

	// Function to start or pause the simulation:
	function startStop() {
		running = !running;
		if (running) {
			startButton.value = "Pause";
			resetTimer();
			simulate();
		} else {
			startButton.value = " Run ";
		}
	}

	// Reset the timer that handles performance evaluation:
	function resetTimer() {
		stepCount = 0;
		startTime = (new Date()).getTime();
	}

	// Show value of flow speed setting:
	function adjustSpeed() {
		speedValue.innerHTML = Number(speedSlider.value).toFixed(3);
	}

	// Show value of viscosity:
	function adjustViscosity() {
		viscValue.innerHTML = Number(viscSlider.value).toFixed(3);
	}

	// Show or hide the data area:
	function showData() {
		if (dataCheck.checked) {
			dataSection.style.display="block";
		} else {
			dataSection.style.display="none";
		}
	}

	// Start or stop collecting data:
	function startOrStopData() {
		collectingData = !collectingData;
		if (collectingData) {
			time = 0;
			dataArea.innerHTML = "Time \tDensity\tVel_x \tVel_y \tForce_x\tForce_y\n";
			writeData();
			dataButton.value = "Stop data collection";
			showingPeriod = false;
			periodButton.value = "Show F_y period";
		} else {
			dataButton.value = "Start data collection";
		}
	}

	// Write one line of data to the data area:
	function writeData() {
		var timeString = String(time);
		while (timeString.length < 5) timeString = "0" + timeString;
		sIndex = sensorX + sensorY*xdim;
		dataArea.innerHTML += timeString + "\t" + Number(rho[sIndex]).toFixed(4) + "\t"
			+ Number(ux[sIndex]).toFixed(4) + "\t" + Number(uy[sIndex]).toFixed(4) + "\t"
			+ Number(barrierFx).toFixed(4) + "\t" + Number(barrierFy).toFixed(4) + "\n";
		dataArea.scrollTop = dataArea.scrollHeight;
	}

	// Handle click to "show period" button
	function showPeriod() {
		showingPeriod = !showingPeriod;
		if (showingPeriod) {
			time = 0;
			lastBarrierFy = 1.0;	// arbitrary positive value
			lastFyOscTime = -1.0;	// arbitrary negative value
			dataArea.innerHTML = "Period of F_y oscillation\n";
			periodButton.value = "Stop data";
			collectingData = false;
			dataButton.value = "Start data collection";
		} else {
			periodButton.value = "Show F_y period";
		}
	}

	// Write all the barrier locations to the data area:
	function showBarrierLocations() {
		dataArea.innerHTML = '{name:"Barrier locations",\n';
		dataArea.innerHTML += 'locations:[\n';
		for (var y=1; y<ydim-1; y++) {
			for (var x=1; x<xdim-1; x++) {
				if (barrier[x+y*xdim]) dataArea.innerHTML += x + ',' + y + ',\n';
			}
		}
		dataArea.innerHTML = dataArea.innerHTML.substr(0, dataArea.innerHTML.length-2); // remove final comma
		dataArea.innerHTML += '\n]},\n';
	}

	// Place a preset barrier:
	function placePresetBarrier() {
		var index = barrierSelect.selectedIndex;
		if (index == 0) return;
		clearBarriers();
		var bCount = barrierList[index-1].locations.length/2;	// number of barrier sites
		// To decide where to place it, find minimum x and min/max y:
		var xMin = barrierList[index-1].locations[0];
		var yMin = barrierList[index-1].locations[1];
		var yMax = yMin;
		for (var siteIndex=2; siteIndex<2*bCount; siteIndex+=2) {
			if (barrierList[index-1].locations[siteIndex] < xMin) {
				xMin = barrierList[index-1].locations[siteIndex];
			}
			if (barrierList[index-1].locations[siteIndex+1] < yMin) {
				yMin = barrierList[index-1].locations[siteIndex+1];
			}
			if (barrierList[index-1].locations[siteIndex+1] > yMax) {
				yMax = barrierList[index-1].locations[siteIndex+1];
			}
		}
		var yAverage = Math.round((yMin+yMax)/2);
		// Now place the barriers:
		for (var siteIndex=0; siteIndex<2*bCount; siteIndex+=2) {
			var x = barrierList[index-1].locations[siteIndex] - xMin + Math.round(ydim/3);
			var y = barrierList[index-1].locations[siteIndex+1] - yAverage + Math.round(ydim/2);
			addBarrier(x, y);
		}
		paintCanvas();
		barrierSelect.selectedIndex = 0;	// A choice on this menu is a one-time action, not an ongoing setting
	}

	// Print debugging data:
	function debug() {
		dataArea.innerHTML = "Tracer locations:\n";
		for (var t=0; t<nTracers; t++) {
			dataArea.innerHTML += tracerX[t] + ", " + tracerY[t] + "\n";
		}
	}
</script>
    
</body>
</html>
 */
