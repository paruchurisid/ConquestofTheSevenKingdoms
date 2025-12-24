import math
import time

class Faction:
    def __init__(self, name, color, passive_type=None):
        self.name = name
        self.color = color
        self.passive_type = passive_type  # e.g., 'speed', 'start_count'

class Castle:
    def __init__(self, x, y, size, owner=None, unit_count=0):
        self.x = x
        self.y = y
        self.size = size  # Radius, also determines regeneration rate
        self.owner = owner # Faction object or None
        self.unit_count = unit_count
        self.last_regen_time = time.time()

        # Apply starting count passive if applicable
        if self.owner and self.owner.passive_type == 'start_count':
            self.unit_count += 20  # Bonus starting units

    def update(self, current_time):
        if self.owner:
            # Regen logic: bigger castles regen faster.
            regen_delay = 50.0 / self.size

            # Apply regen_bonus passive (Baratheon)
            if self.owner.passive_type == 'regen_bonus':
                regen_delay *= 0.7  # 30% faster regeneration

            if current_time - self.last_regen_time >= regen_delay:
                self.unit_count += 1
                self.last_regen_time = current_time

        # Cap unit count? Maybe 3 * size?
        if self.unit_count > self.size * 5:
            self.unit_count = self.size * 5

    def receive_army(self, army):
        if army.owner == self.owner:
            self.unit_count += army.count
        else:
            damage = army.count
            # Apply defense_bonus passive (Stark)
            if self.owner and self.owner.passive_type == 'defense_bonus':
                # Defenders are stronger, so attackers count for less?
                # Or defenders die slower. Let's say incoming damage is reduced.
                damage = int(damage * 0.75) # 25% damage reduction

            self.unit_count -= damage
            if self.unit_count < 0:
                self.owner = army.owner
                self.unit_count = abs(self.unit_count)
                # Reset regen timer on capture?
                self.last_regen_time = time.time()

class Army:
    def __init__(self, owner, count, start_pos, target_castle, speed=50):
        self.owner = owner
        self.count = count
        self.x, self.y = start_pos
        self.target_castle = target_castle
        self.speed = speed

        # Apply speed passive
        if self.owner and self.owner.passive_type == 'speed':
            self.speed *= 1.5

    def update(self, dt):
        # Move towards target
        dx = self.target_castle.x - self.x
        dy = self.target_castle.y - self.y
        dist = math.hypot(dx, dy)

        if dist < 5: # Arrived
            return True

        move_dist = self.speed * dt
        self.x += (dx / dist) * move_dist
        self.y += (dy / dist) * move_dist
        return False

class GameState:
    def __init__(self):
        self.castles = []
        self.armies = []
        self.factions = {}
        self.last_update_time = time.time()

    def add_faction(self, faction):
        self.factions[faction.name] = faction

    def add_castle(self, castle):
        self.castles.append(castle)

    def spawn_army(self, source_castle, target_castle):
        if source_castle.unit_count < 2:
            return # Can't send if too few units

        # Send half units? Or all minus 1? Let's say half.
        # State.io typically sends half by default on drag, or continuous stream.
        # Let's implement "Send Half" for a single drag.
        count_to_send = source_castle.unit_count // 2
        source_castle.unit_count -= count_to_send

        army = Army(source_castle.owner, count_to_send, (source_castle.x, source_castle.y), target_castle)
        self.armies.append(army)

    def update(self):
        current_time = time.time()
        dt = current_time - self.last_update_time
        self.last_update_time = current_time

        for castle in self.castles:
            castle.update(current_time)

        # Update armies and check for arrival
        armies_to_remove = []
        for army in self.armies:
            arrived = army.update(dt)
            if arrived:
                army.target_castle.receive_army(army)
                armies_to_remove.append(army)

        for army in armies_to_remove:
            self.armies.remove(army)
