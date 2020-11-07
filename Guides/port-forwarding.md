# How to forward https port

## Linux

```sh
sudo sysctl net.ipv4.ip_forward=1
sudo sysctl -w net.ipv4.conf.eth0.route_localnet=1
sudo iptables -t nat -A PREROUTING -p tcp --dport 443 -j DNAT --to-destination 127.0.0.1:5001
sudo iptables -t nat -A POSTROUTING -j MASQUERADE
sudo sysctl -p
# to check
sudo iptables -t nat -L
```
