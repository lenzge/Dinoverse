import sys
#sys.path.append('C:\\Users\\Lena Sophie\\Desktop\\Game Dev\\Dinoverse\\venv\\Lib\\site-packages')
import neat
import os
from itertools import count

class AnimalBrain:
    def __init__(self, genome, config):
        self.config = config
        self.genome = genome
        self.brain = neat.nn.FeedForwardNetwork.create(genome, config)
        self.genome_indexer = count(1)
        self.complexity = genome.size()

    def survive(self, position_x, position_z, food_position_x, food_position_y):
        output = self.brain.activate([position_x, position_z, food_position_x, food_position_y])
        return output

    def update_fitness(self, fitness):
        self.genome.fitness += fitness

    def reproduce(self, partner):
        gid = next(self.genome_indexer)
        child = self.config.genome_type(gid)
        child.configure_crossover(self.genome, partner.genome, self.config.genome_config)
        child.mutate(self.config.genome_config)
        child.fitness = 0
        new_brain = AnimalBrain(child, self.config)
        return new_brain

    def return_genome(self):
        return self.genome

    def return_fitness(self):
        return self.genome.fitness


class NeatController:
    def __init__(self):
        self.animals = []
        self.population = None

    def eval_genomes(self, genomes, config):
        for genome_id, genome in genomes:
            genome.fitness = 0
            self.animals.append(AnimalBrain(genome, config))

    def run(self):
        # Load configuration
        local_dir = os.path.dirname(__file__)
        config_file = os.path.join(local_dir, "config.txt")

        config = neat.Config(neat.DefaultGenome, neat.DefaultReproduction,
                             neat.DefaultSpeciesSet, neat.DefaultStagnation, config_file)

        # Create the population (or load from a checkpoint), which is the top-level object for a NEAT run
        #population = neat.Checkpointer.restore_checkpoint('neat-checkpoint-27') 
        self.population = neat.Population(config)

        # Add a stdout reporter to show progress in the terminal
        self.population.add_reporter(neat.StdOutReporter(True))
        stats = neat.StatisticsReporter()
        self.population.add_reporter(stats)
        #population.add_reporter(neat.Checkpointer(10))

        # Run 
        #winner = self.population.run(self.eval_genomes, 3)

    def create_generation(self):
        self.animals = []
        self.population.create_population(self.eval_genomes)

    def evaluate(self):
        generationwinner = self.population.evaluate_generation()
        return generationwinner

    def return_winner(self):
        return self.population.best_genome

