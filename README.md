# panoramic-meta-generator

This project generates XML data for panoramic video.

##Panoramic video recording
[code](https://gist.github.com/se0kjun/f4b0fdf395181b495f79)

##Data Format
```
<?xml version="1.0" encoding="UTF-8" ?>
<data>
	<videos>
		<video seq="" frame="" width="" height="">file_name</video>
		...
	</videos>
	<markers>
		<marker id="">
			<track video="" position_x="" position_y="" frame=""></track>
			...
		</marker>
		...
	</markers>
</data>
```

##Requirements

- [OpenCvSharp3](https://www.nuget.org/packages/OpenCvSharp3-AnyCPU/3.0.0.20150919)
- [OpenCV.Net](https://www.nuget.org/packages/OpenCV.Net/3.3.0)
- [Aruco.Net](https://www.nuget.org/packages/Aruco.Net/2.0.0)