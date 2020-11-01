# How to forward https port

## Linux

```sh
sudo sysctl net.ipv4.ip_forward=1
iptables -t nat -A PREROUTING -p tcp --dport 80 -j DNAT --to-destination 127.0.0.1:5001
iptables -t nat -A POSTROUTING -j MASQUERADE
```
