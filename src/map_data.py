from src.model import GameState, Faction, Castle

# Coordinates are roughly mapped to a 800x600 screen for now, centered on Westeros shape.
# (0,0) top left.

def create_initial_map():
    game = GameState()

    # Factions
    stark = Faction("Stark", (200, 200, 200), passive_type="defense_bonus") # Grey
    lannister = Faction("Lannister", (220, 20, 60), passive_type="start_count") # Crimson
    targaryen = Faction("Targaryen", (0, 0, 0), passive_type="speed") # Black (with red trim usually, but black for core)
    baratheon = Faction("Baratheon", (255, 215, 0), passive_type="regen_bonus") # Gold

    game.add_faction(stark)
    game.add_faction(lannister)
    game.add_faction(targaryen)
    game.add_faction(baratheon)

    # Castles (Nodes)
    # Winterfell (North)
    game.add_castle(Castle(400, 100, 40, owner=stark, unit_count=20))

    # The Twins (Central/North) - Neutral
    game.add_castle(Castle(380, 250, 25, owner=None, unit_count=10))

    # The Eyrie (East) - Neutral
    game.add_castle(Castle(550, 280, 30, owner=None, unit_count=15))

    # Riverrun (Central) - Neutral
    game.add_castle(Castle(350, 320, 25, owner=None, unit_count=10))

    # Casterly Rock (West)
    game.add_castle(Castle(150, 350, 40, owner=lannister, unit_count=30)) # Lannister bonus handled in __init__ if owner passed?
    # Note: In model.py I put logic in __init__ to add bonus. So if I pass 30 here, it might become 50.

    # King's Landing (South East)
    game.add_castle(Castle(500, 450, 45, owner=baratheon, unit_count=20))

    # Highgarden (South West) - Neutral
    game.add_castle(Castle(250, 500, 35, owner=None, unit_count=20))

    # Dragonstone (East Island)
    game.add_castle(Castle(650, 350, 35, owner=targaryen, unit_count=20))

    # Sunspear (Deep South) - Neutral
    game.add_castle(Castle(450, 580, 30, owner=None, unit_count=15))

    return game
