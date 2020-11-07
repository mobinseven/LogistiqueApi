# -*- coding: utf-8 -*-
"""
v3.1
Created on Mon Mar 30 10:15:48 2020

@author: sabersk
"""


def solver(file_address):
    import json
    import io
    import pickle
    import os

    Route = {}
    with open(file_address,encoding="utf8") as datafile:
        data = json.load(datafile) 
    vehicle_ID = list(data['vehicle'].keys())
    request_ID = list(data['request'].keys())
    request_ID.insert(0, data['vehicle'][vehicle_ID[0]]['start_location_id'])
    # Send the problem to server
    try:
        os.system(
            'cd /home/htlg/Codes_test_v3/Simulator_organized ; python3 Simulatorjson.py ' + file_address)
        with open('/home/htlg/Codes_test_v3/Simulator_organized/website/output_routes', 'rb') as fp:
            Output = pickle.load(fp)
        if Output[0] == 'None':
            Status = 200
        else:
            Status = 100
        R = Output[0]
        Route['PlanId'] = data['plan-id']
        Route['Status'] = Status
        if Route['Status'] == 100:
            T = Output[2]
            D = Output[3]
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
                    if ocnt < len(r)-1:
                        o['distanceToNext'] = D[cnt][ocnt]
                    else:
                        o['distanceToNext'] = 0
                    O.append(o)
                    ocnt = ocnt+1
                route['driverId'] = vehicle_ID[cnt]
                route['route'] = O
                routes.append(route)
                cnt = cnt+1
            Route['Routes'] = routes
    except Exception as e:
        print(e)
        with open('error.txt', 'w') as f:
            f.write(e)
    finally:
        return Route