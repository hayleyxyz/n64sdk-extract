bin/Debug/netcoreapp2.0/n64sdk-extract.dll: obj/project.assets.json
	msbuild

obj/project.assets.json:
	nuget restore

.PHONY: clean
clean:
	rm -rf bin/ obj/
