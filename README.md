# panoramic-meta-generator

This project generates XML data for panoramic video.

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

