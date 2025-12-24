import random
import math

class AIController:
    def __init__(self, game_state):
        self.game_state = game_state
        self.last_action_time = 0
        self.action_interval = 2.0 # AI acts every 2 seconds

    def update(self, current_time):
        if current_time - self.last_action_time < self.action_interval:
            return

        self.last_action_time = current_time

        # Simple AI Logic:
        # Iterate through all factions that are NOT controlled by human (assuming Stark is human)
        # For this prototype, let's assume Stark is Human, others are AI.

        # We need to know who is human. For now, hardcoded: Stark is Human.
        human_faction_name = "Stark"

        ai_factions = [f for name, f in self.game_state.factions.items() if name != human_faction_name]

        for faction in ai_factions:
            self.take_turn(faction)

    def take_turn(self, faction):
        # 1. Get all castles owned by this faction
        my_castles = [c for c in self.game_state.castles if c.owner == faction]

        for castle in my_castles:
            # If castle has enough units, try to attack or reinforce
            if castle.unit_count > 10:
                # Find a target
                # Priority:
                # 1. Weak neutral castles nearby
                # 2. Weak enemy castles nearby
                # 3. Reinforce own castles that are threatened (not implemented yet)

                target = self.find_best_target(castle, faction)
                if target:
                    # Random chance to actually attack to simulate reaction time/imperfection
                    if random.random() < 0.7:
                         self.game_state.spawn_army(castle, target)

    def find_best_target(self, source_castle, faction):
        potential_targets = []

        for other in self.game_state.castles:
            if other == source_castle:
                continue

            dist = math.hypot(other.x - source_castle.x, other.y - source_castle.y)
            if dist > 300: # Too far
                continue

            score = 0

            # Distance factor (closer is better)
            score -= dist * 0.1

            if other.owner is None:
                # Neutral: Easy picking if weak
                if source_castle.unit_count > other.unit_count + 5:
                    score += 50
            elif other.owner != faction:
                # Enemy
                if source_castle.unit_count > other.unit_count + 10:
                    score += 60 # High priority to take enemy land
                elif source_castle.unit_count < other.unit_count:
                    score -= 50 # Don't suicide
            else:
                # Own castle (Reinforce)
                if other.unit_count < 10:
                    score += 20

            potential_targets.append((score, other))

        # Sort by score
        potential_targets.sort(key=lambda x: x[0], reverse=True)

        if potential_targets and potential_targets[0][0] > 0:
            return potential_targets[0][1]
        return None
