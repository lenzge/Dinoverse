import sys
sys.path.append('C:\\Users\\Lena Sophie\\Desktop\\Game Dev\\Dinoverse\\Packages\\python_net\\Lib\\site-packages')
import neat
import os


class AnimalController:
    def __init__(self, brain):
        self.brain = brain

    def survive(self, position_x, position_z, food_position_x, food_position_y):
        output = self.brain.activate([position_x, position_z, food_position_x, food_position_y])
        return output


class NeatController:
    def __init__(self):
        self.animals = []

    def eval_genomes(self, genomes, config):
        for genome_id, genome in genomes:
            genome.fitness = 0
            brain = neat.nn.FeedForwardNetwork.create(genome, config)
            self.animals.append(AnimalController(brain))


    def run(self):
        # Load configuration
        local_dir = os.path.dirname(__file__)
        config_file = os.path.join(local_dir, "config.txt")
    
        config = neat.Config(neat.DefaultGenome, neat.DefaultReproduction,
                             neat.DefaultSpeciesSet, neat.DefaultStagnation, config_file)
    
        # Create the population (or load from a checkpoint), which is the top-level object for a NEAT run
        #population = neat.Checkpointer.restore_checkpoint('neat-checkpoint-27') 
        population = neat.Population(config)
    
        # Add a stdout reporter to show progress in the terminal
        population.add_reporter(neat.StdOutReporter(True))
        stats = neat.StatisticsReporter()
        population.add_reporter(stats)
        population.add_reporter(neat.Checkpointer(1))
    
        # Run for up to 50 generations
        winner = population.run(self.eval_genomes, 50)
