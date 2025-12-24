import pytest
import time
from src.model import GameState, Faction, Castle, Army

def test_faction_init():
    f = Faction("Test", (255, 255, 255), "speed")
    assert f.name == "Test"
    assert f.color == (255, 255, 255)
    assert f.passive_type == "speed"

def test_castle_regeneration():
    f = Faction("Test", (0,0,0))
    # Size 50 -> regen_delay = 1.0s
    c = Castle(0, 0, 50, owner=f, unit_count=10)

    start_time = time.time()
    c.last_regen_time = start_time - 1.1 # Force regen
    c.update(start_time)

    assert c.unit_count == 11

def test_castle_combat_friendly():
    f = Faction("Test", (0,0,0))
    c = Castle(0, 0, 10, owner=f, unit_count=10)
    a = Army(f, 5, (0,0), c)

    c.receive_army(a)
    assert c.unit_count == 15
    assert c.owner == f

def test_castle_combat_enemy_defense():
    f1 = Faction("F1", (0,0,0))
    f2 = Faction("F2", (1,1,1))
    c = Castle(0, 0, 10, owner=f1, unit_count=10)
    a = Army(f2, 5, (0,0), c)

    c.receive_army(a)
    assert c.unit_count == 5
    assert c.owner == f1

def test_castle_combat_capture():
    f1 = Faction("F1", (0,0,0))
    f2 = Faction("F2", (1,1,1))
    c = Castle(0, 0, 10, owner=f1, unit_count=5)
    a = Army(f2, 10, (0,0), c)

    c.receive_army(a)
    # 5 - 10 = -5 -> abs(-5) = 5
    assert c.unit_count == 5
    assert c.owner == f2

def test_passive_start_count():
    f = Faction("Lannister", (0,0,0), passive_type='start_count')
    # Normal init is 0 units, but with start_count +20
    c = Castle(0,0, 10, owner=f, unit_count=10)
    assert c.unit_count == 30

def test_passive_speed():
    f = Faction("Targaryen", (0,0,0), passive_type='speed')
    c_source = Castle(0,0,10, owner=f)
    c_target = Castle(100,0,10)
    a = Army(f, 10, (0,0), c_target, speed=100)

    # Base speed 100, passive * 1.5 = 150
    assert a.speed == 150

def test_passive_defense_bonus():
    f1 = Faction("Stark", (0,0,0), passive_type='defense_bonus')
    f2 = Faction("Attacker", (1,1,1))
    c = Castle(0,0, 10, owner=f1, unit_count=20)
    a = Army(f2, 10, (0,0), c)

    # Damage should be 10 * 0.75 = 7.5 -> 7
    # Result should be 20 - 7 = 13
    c.receive_army(a)
    assert c.unit_count == 13

def test_passive_regen_bonus():
    f = Faction("Baratheon", (0,0,0), passive_type='regen_bonus')
    # Size 50 -> Base delay 1.0s. With bonus (*0.7) -> 0.7s
    c = Castle(0, 0, 50, owner=f, unit_count=10)

    start_time = time.time()
    # If we advance 0.8s, it should have regenerated (0.8 > 0.7)
    # Without bonus, it would need 1.0s
    c.last_regen_time = start_time - 0.8
    c.update(start_time)

    assert c.unit_count == 11
