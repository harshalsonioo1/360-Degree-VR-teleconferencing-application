# MM802
360- Degree video teleconferencing application. 

We developed an android application that can be downloaded in APK format and ran in any cell-phone. It allows users to play Live-streamed or pre-downloaded 360-degree VR videos using the native player of the app conected to any Google cardboard. Another way around, just play it on the cell-phone using our full-screen player.  

1 Requirements
Matlab(R2015a or higher) with Parallel Computing Toolbox and Image Processing Toolbox;
Any C++ IDE(Visual Studio 2015 is recommended);
FFMPEG(add the binary folder, e.g. “C:\Program Files\ffmpeg-20190625-dd662bb-win64-static\bin”, to system PATH.)
Python v3.6
Java version v8 with JDK 
PurplePill Unity3D SDK ( Can be downloaded from Google drive by clicking the link for APK file. )

Pre-processing: 
1. Download the PANO's source code from the link provided. 
  Steps for pre-processing:
  1)  Prepare the programs and data
    All the Matlab programs are in root folder “/”. The videos are in the folder “/videos”, the trajectory data is in two format is in “/traj” and “viewpoint”, the pre-calculated tile depth data is in “/DepthMap”. 
  2)  cut chunks
    run “/cutChunk.m” to cut the videos to one-second chunks in “/videos”.
  3)  calculate tile valueness
    set the variable calcTileVal to 1 in “/main.m”, then run “/main.m” to calculate the valueness of each tile in each chunk. The result is stored in “/ratio”.
  4)  generate tiling scheme
    if you’re using Visual Studio 2015, open the project file “/tilingDP/tilingDP.sln” and run it, otherwise you should build a new project in your IDE and copy “/tilingDP/main.cpp” into it, modify the data path at line 254/358/380 as appropriate. The tiling scheme is stored in “/tiling1004”.
  5)  run the simulation
    set the variable calcTileVal to 0 in “/main.m”, then run “/main.m”. The results about PSPNR and bitrate consumption per user per chunk are stored in “/baselineResult” and “/PanoResult” for further analyzation. “/main.m” visualizes a comparison between baseline and Pano in Matlab window.

