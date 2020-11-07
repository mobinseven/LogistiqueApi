## Run with Python on Linux

Run while setting `LD_LIBRARY_PATH`:

```shell
sudo apt-get install libpython3.6
LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/usr/lib/python3.6/config-3.6m-x86_64-linux-gnu/ dotnet LogistiqueApi.dll
```
