﻿using UnityEngine;

namespace Animal
{
    public class Brain : Organ
    {
        public int[] NetworkShape;
        public Layer[] Layers;

        private int stdInputNeurons = 7;
        private int outputNeurons = 7;
        
        public override void Init(bool isChild)
        {
            NetworkShape = CreateNetworkShape();

            if (!isChild)
            {
                Layers = new Layer[NetworkShape.Length - 1];

                for(int i = 0; i < Layers.Length; i++)
                {
                    Layers[i] = new Layer(NetworkShape[i], NetworkShape[i+1]);
                }
            }

            //This ensures that the random numbers we generate aren't the same pattern each time. 
            Random.InitState((int)System.DateTime.Now.Ticks);
        }

        //This function is used to feed forward the inputs through the network, and return the output, which is the decision of the network, in this case, the direction to move in.
        public float[] Survive(float [] inputs)
        {
            for(int i = 0; i < Layers.Length; i++)
            {
                if(i == 0)
                {
                    Layers[i].Forward(inputs);
                    Layers[i].Activation();
                } 
                else if(i == Layers.Length - 1)
                {
                    Layers[i].Forward(Layers[i - 1].Nodes);
                }
                else
                {
                    Layers[i].Forward(Layers[i - 1].Nodes);
                    Layers[i].Activation();
                }    
            }

            return(Layers[Layers.Length - 1].Nodes);
        }

        //This function is used to copy the weights and biases from one neural network to another.
        public Layer[] CopyLayers()
        {
            Layer[] tmpLayers = new Layer[NetworkShape.Length - 1];
            for(int i = 0; i < Layers.Length; i++)
            {
                tmpLayers[i] = new Layer(NetworkShape[i], NetworkShape[i+1]);
                System.Array.Copy (Layers[i].Weights, tmpLayers[i].Weights, Layers[i].Weights.GetLength(0)*Layers[i].Weights.GetLength(1));
                System.Array.Copy (Layers[i].Biases, tmpLayers[i].Biases, Layers[i].Biases.GetLength(0));
            }
            return tmpLayers;
        }
        
        //Call the randomness function for each layer in the network.
        public void MutateNetwork()
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            for(int i = 0; i < Layers.Length; i++)
            {
                Layers[i].MutateLayer(animalController.DNA.MutationChance[0], animalController.DNA.MutationAmount[0]);
            }
        }
        
        public void CrossoverNetwork(Brain other)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            
            for(int i = 0; i < Layers.Length; i++)
            {
                Layers[i].CrossoverLayer(other.Layers[i]);
            }
        }

        private int[] CreateNetworkShape()
        {
            switch (animalController.DNA.HiddenLayer[0])
            {
                case 2:
                    return new []{animalController.DNA.NumRaycasts[0]*7 + stdInputNeurons, animalController.DNA.MaxNeurons[0], animalController.DNA.MinNeurons[0], outputNeurons};
                case 3:
                    return new []{animalController.DNA.NumRaycasts[0]*7 + stdInputNeurons, animalController.DNA.MaxNeurons[0], animalController.DNA.MaxNeurons[0], animalController.DNA.MinNeurons[0], outputNeurons};
                /*case 4:
                    return new []{DNA.NumRaycasts[0]*2 +stdInputNeurons, DNA.MinNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MinNeurons[0], outputNeurons};
                case 5:
                    return new []{DNA.NumRaycasts[0]*2 +stdInputNeurons, DNA.MinNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MinNeurons[0], outputNeurons};
                case 6:
                    return new []{DNA.NumRaycasts[0]*2 +stdInputNeurons, DNA.MinNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MinNeurons[0], outputNeurons};
                case 7:
                    return new []{DNA.NumRaycasts[0]*2 +stdInputNeurons, DNA.MinNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MinNeurons[0], outputNeurons};
                case 8:
                    return new []{DNA.NumRaycasts[0]*2 +stdInputNeurons, DNA.MinNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MinNeurons[0], outputNeurons};
                case 9:
                    return new []{DNA.NumRaycasts[0]*2 +stdInputNeurons, DNA.MinNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MinNeurons[0], outputNeurons};
                case 10:
                    return new []{DNA.NumRaycasts[0]*2 +stdInputNeurons, DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], DNA.MaxNeurons[0], outputNeurons};
                */
                default:
                    return new []{animalController.DNA.NumRaycasts[0]*7 + stdInputNeurons, animalController.DNA.MaxNeurons[0], animalController.DNA.MinNeurons[0], outputNeurons};
            }
        }

        public class Layer
        {
            public float[,] Weights;
            public float[] Biases;
            public float [] Nodes;

            public int Inputs;
            public int Neurons;
            
            public Layer(int inputs, int neurons)
            {
                this.Inputs = inputs;
                this.Neurons = neurons;

                Weights = new float [neurons, inputs];
                Biases = new float[neurons];
            }

            //forward pass, takes in an array of inputs and returns an array of outputs, which is then used as the input
            //for the next layer, and so on, until we get to the output layer, which is returned as the output of the network.
            public void Forward(float [] inputsArray)
            {
                Nodes = new float [Neurons];

                for(int i = 0;i < Neurons ; i++)
                {
                    //sum of weights times inputs
                    for(int j = 0; j < Inputs; j++)
                    {
                        Nodes[i] += Weights[i,j] * inputsArray[j];
                    }

                    //add the bias
                    Nodes[i] += Biases[i];
                }
            }

            //This function is the activation function for the neural network uncomment the one you want to use.
            public void Activation()
            {
                // //leaky relu function
                // for(int i = 0; i < nodeArray.Length; i++)
                // {
                //     if(nodeArray[i] < 0)
                //     {
                //         nodeArray[i] = nodeArray[i]/10;
                //     }
                // }


                // //sigmoid function
                // for(int i = 0; i < nodeArray.Length; i++)
                // {
                //     nodeArray[i] = 1/(1 + Mathf.Exp(-nodeArray[i]));
                // }

                //tanh function
                for(int i = 0; i < Nodes.Length; i++)
                {
                    Nodes[i] = (float)System.Math.Tanh(Nodes[i]);
                }

                // //relu function
                // for(int i = 0; i < nodeArray.Length; i++)
                // {
                //     if(nodeArray[i] < 0)
                //     {
                //         nodeArray[i] = 0;
                //     }
                // }
            }

            //This is used to randomly modify the weights and biases for the Evolution Sim and Genetic Algorithm.
            public void MutateLayer(float mutationChance, float mutationAmount)
            {
                for(int i = 0; i < Neurons; i++)
                {
                    for(int j = 0; j < Inputs; j++)
                    {
                        if(Random.value < mutationChance)
                        {
                            Weights[i,j] += Random.Range(-1.0f, 1.0f)*mutationAmount;
                        }
                    }

                    if(Random.value < mutationChance)
                    {
                        Biases[i] += Random.Range(-1.0f, 1.0f)*mutationAmount;
                    }
                }
            }
            
            public void CrossoverLayer(Layer other)
            {
                for(int i = 0; i < Neurons; i++)
                {
                    for(int j = 0; j < Inputs; j++)
                    {
                        if(Random.value <= 0.5)
                        {
                            Weights[i,j] =other.Weights[i,j];
                        }
                    }

                    if(Random.value <= 0.5)
                    {
                        Biases[i] = other.Biases[i];
                    }
                }
            }
        }
    }
}