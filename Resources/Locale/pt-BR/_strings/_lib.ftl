### Special messages used by internal localizer stuff.

# Used internally by the PRESSURE() function.
zzzz-fmt-pressure = { TOSTRING($divided, "F1") } { $places ->
    [0] kPa
    [1] MPa
    [2] GPa
    [3] TPa
    [4] PBa
    *[5] ???
}

# Usado internamente pela função POWERWATTS ().
zzzz-fmt-power-watts = { TOSTRING($divided, "F1") } { $places ->
    [0] W
    [1] kW
    [2] MW
    [3] GW
    [4] TW
    *[5] ???
}

# Usado internamente pela função POWERJOULES ().
# Lembrete: 1 joule = 1 watt por 1 segundo (multiplica watts por segundos para obter joules).
# Portanto, 1 quilowatt-hora é igual a 3.600.000 joules (3.6MJ)
zzzz-fmt-power-joules = { TOSTRING($divided, "F1") } { $places ->
    [0] J
    [1] kJ
    [2] MJ
    [3] GJ
    [4] TJ
    *[5] ???
}

# Usado internamente pela função ENERGYWATTHOURS ().
zzzz-fmt-energy-watt-hours = { TOSTRING($divided, "F1") } { $places ->
    [0] Wh
    [1] kWh
    [2] MWh
    [3] GWh
    [4] TWh
    *[5] ???
}

# Usado internamente pela função PLAYTIME ().
zzzz-fmt-playtime = { $hours }H { $minutes }M
