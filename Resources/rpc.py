# -*- coding: utf-8 -*-
"""
v3.1
Created on Mon Mar 30 10:15:48 2020

@author: sabersk
"""


def solver(strjson):
    import xmlrpc.client
    import json
    import io

    Route = {}

    data = json.loads(strjson)
    vehicle_ID = list(data['vehicle'].keys())
    request_ID = list(data['request'].keys())
    request_ID.insert(0,data['vehicle'][vehicle_ID[0]]['start_location_id'])
    # Send the problem to server
    try:
        with xmlrpc.client.ServerProxy("http://213.233.161.118:8000/") as proxy:
            Output = proxy.processjson(strjson)
            R = Output[0]
            Route['PlanId'] = data['plan-id']
            Route['Status'] = Output[1]
            if Route['Status'] == 100:
                T = Output[2]
                routes = []
                cnt = 0
                for r in R:
                    route = {}
                    O = []
                    ocnt = 0
                    for v in r:
                        o = {}
                        o['order'] = ocnt
                        o['requestId'] = request_ID[v]
                        o['time'] = T[cnt][ocnt]
                        O.append(o)
                        ocnt = ocnt+1
                    route['driverId'] = vehicle_ID[cnt]
                    route['route'] = O
                    routes.append(route)
                    cnt = cnt+1
                Route['Routes'] = routes
    except ConnectionError:
        Route['Status'] = 503
    finally:
        return Route
