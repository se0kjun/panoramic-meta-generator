# panoramic-meta-generator

This project generates XML data for panoramic video.

##Panoramic video recording
[code](https://gist.github.com/se0kjun/f4b0fdf395181b495f79)

##Data Format
```
<?xml version="1.0" encoding="UTF-8" ?>
<videos>
	<video seq="" angle="">file_name</video>
	...
</videos>
<markers>
	<marker id="">
		<track video="" position_x="" position_y="" frame=""></track>
		...
	</marker>
	...
</markers>
```

