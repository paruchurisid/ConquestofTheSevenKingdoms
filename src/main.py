import pygame
import math
from src.map_data import create_initial_map
from src.ai import AIController
import time

# Constants
SCREEN_WIDTH = 800
SCREEN_HEIGHT = 600
BG_COLOR = (245, 222, 179) # Wheat/Parchment color
TEXT_COLOR = (50, 50, 50)

class Renderer:
    def __init__(self, screen):
        self.screen = screen
        self.font = pygame.font.SysFont("Arial", 20, bold=True)
        self.small_font = pygame.font.SysFont("Arial", 14)

    def draw_game(self, game_state):
        self.screen.fill(BG_COLOR)

        # Draw connections (roads) ? Optional, maybe later.

        # Draw Castles
        for castle in game_state.castles:
            color = (150, 150, 150) # Neutral grey
            if castle.owner:
                color = castle.owner.color

            # Draw 'wax seal' effect (border)
            pygame.draw.circle(self.screen, (100, 100, 100), (int(castle.x), int(castle.y)), castle.size + 2)
            pygame.draw.circle(self.screen, color, (int(castle.x), int(castle.y)), castle.size)

            # Draw Unit Count
            text_surf = self.font.render(str(int(castle.unit_count)), True, (255, 255, 255))
            text_rect = text_surf.get_rect(center=(castle.x, castle.y))
            self.screen.blit(text_surf, text_rect)

            # Draw Castle Name/Icon? For now, simplistic.

        # Draw Armies
        for army in game_state.armies:
            color = army.owner.color if army.owner else (50, 50, 50)
            # Draw as small circles or triangles
            # Moving towards target
            pygame.draw.circle(self.screen, color, (int(army.x), int(army.y)), 5)

            # Draw count above army
            count_surf = self.small_font.render(str(army.count), True, (0, 0, 0))
            self.screen.blit(count_surf, (army.x + 5, army.y - 10))

        # Draw "War Room" UI elements
        # Maybe a banner at top?

    def draw_drag_line(self, start_pos, end_pos, color):
        pygame.draw.line(self.screen, color, start_pos, end_pos, 3)

def main():
    pygame.init()
    screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
    pygame.display.set_caption("Conquest of the Seven Kingdoms")
    clock = pygame.time.Clock()

    game_state = create_initial_map()
    renderer = Renderer(screen)
    ai_controller = AIController(game_state)

    running = True
    selected_castle = None
    drag_active = False

    # Identify Player Faction (Let's say Player is Stark for now)
    # Ideally, we should pick one or have a menu.
    player_faction = game_state.factions["Stark"]

    while running:
        dt = clock.tick(60) / 1000.0 # Delta time in seconds

        # Input Handling
        mouse_pos = pygame.mouse.get_pos()

        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False

            elif event.type == pygame.MOUSEBUTTONDOWN:
                if event.button == 1: # Left click
                    # Check if clicked on a player's castle
                    for castle in game_state.castles:
                        dist = math.hypot(mouse_pos[0] - castle.x, mouse_pos[1] - castle.y)
                        if dist <= castle.size:
                            if castle.owner == player_faction:
                                selected_castle = castle
                                drag_active = True
                            break

            elif event.type == pygame.MOUSEBUTTONUP:
                if event.button == 1 and drag_active:
                    # Check if released over another castle
                    target_castle = None
                    for castle in game_state.castles:
                        dist = math.hypot(mouse_pos[0] - castle.x, mouse_pos[1] - castle.y)
                        if dist <= castle.size:
                            target_castle = castle
                            break

                    if target_castle and target_castle != selected_castle:
                        game_state.spawn_army(selected_castle, target_castle)

                    selected_castle = None
                    drag_active = False

        # Update Game Logic
        game_state.update()
        ai_controller.update(time.time())

        # Render
        renderer.draw_game(game_state)

        if drag_active and selected_castle:
            renderer.draw_drag_line((selected_castle.x, selected_castle.y), mouse_pos, player_faction.color)

        pygame.display.flip()

    pygame.quit()

if __name__ == "__main__":
    main()
