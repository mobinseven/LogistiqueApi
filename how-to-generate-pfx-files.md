# How to generate .pfx files
 Taken from [Kimserey's blog](https://kimsereyblog.blogspot.com/2018/07/self-signed-certificate-for-identity.html).

To generate the app .pfx (for the `configuration["certificates:signing"]`) :

 ```sh
sudo openssl req -x509 -newkey rsa:4096 -keyout myapp.key -out myapp.crt -days 3650 -nodes -subj "/CN=myapp"

sudo openssl pkcs12 -export -out myapp.pfx -inkey myapp.key -in myapp.crt -name "Some friendly name"


``` 

To generate the ssl .pfx (for the `configuration["certificates:ssl"]`):

```sh
cp /etc/ssl/openssl.cnf ssl-selfsigned.cnf
```

Then customize the .cnf config file for this purpose:(Taken from [this comment](https://github.com/dotnet/aspnetcore/issues/7246#issuecomment-540998114))

```

[v3_ca]
...
subjectAltName = @alt_names
keyUsage       = keyCertSign, nonRepudiation, digitalSignature, keyEncipherment

[alt_names]
DNS.1   = localhost
DNS.2   = 127.0.0.1
DNS.3   = {ip} # I added this

```

Then generate .crt and .key based on this config:(I used {ip} instead of localhost)

```sh
sudo openssl req -x509 -newkey rsa:4096 -keyout ssl-selfsigned.key -out ssl-selfsigned.crt -days 3650 -nodes -subj "/CN={ip}" -config ssl-selfsigned.cnf
```

Then generate the .pfx based the .crt and .key:
```sh
sudo openssl pkcs12 -export -out ssl.pfx -inkey ssl-selfsigned.key -in ssl-selfsigned.crt -name "Localhost Selfsigned"
```
## Install .crt on Linux

To install the .crt on Linux machine:([this comment](https://github.com/dotnet/aspnetcore/issues/7246#issuecomment-541063133))
```sh
sudo cp ssl-selfsigned.crt /usr/local/share/ca-certificates/ssl-selfsigned.crt
sudo dpkg-reconfigure ca-certificates
```
