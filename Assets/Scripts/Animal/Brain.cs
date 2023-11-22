using UnityEngine;

namespace Animal
{
    public class Brain : Organ
    {
        public DNA DNA;
        
        public int[] NetworkShape;
        public Layer[] Layers;

        private int stdInputNeurons = 3;
        private int outputNeurons = 6;
        
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

            MutateNetwork(DNA.MutationChance[0], DNA.MutationAmount[0]);
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
        public void MutateNetwork(float mutationChance, float mutationAmount)
        {
            for(int i = 0; i < Layers.Length; i++)
            {
                Layers[i].MutateLayer(mutationChance, mutationAmount);
            }
        }

        private int[] CreateNetworkShape()
        {
            switch (DNA.HiddenLayer[0])
            {
                case 1:
                    return new []{DNA.NumRaycasts[0]*2 +stdInputNeurons, DNA.MaxNeurons[0], outputNeurons};
                case 2:
                    return new []{DNA.NumRaycasts[0]*2 +stdInputNeurons, DNA.MaxNeurons[0], DNA.MinNeurons[0], outputNeurons};
                case 3:
                    return new []{DNA.NumRaycasts[0]*2 +stdInputNeurons, DNA.MinNeurons[0], DNA.MaxNeurons[0], DNA.MinNeurons[0], outputNeurons};
                case 4:
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
                default:
                    return new []{DNA.NumRaycasts[0]*2 +stdInputNeurons, DNA.MaxNeurons[0], outputNeurons};
            }
        }

        public class Layer
        {
            public float[,] Weights;
            public float[] Biases;
            public float [] Nodes;

            private int inputs;
            private int neurons;
            
            public Layer(int inputs, int neurons)
            {
                this.inputs = inputs;
                this.neurons = neurons;

                Weights = new float [neurons, inputs];
                Biases = new float[neurons];
            }

            //forward pass, takes in an array of inputs and returns an array of outputs, which is then used as the input
            //for the next layer, and so on, until we get to the output layer, which is returned as the output of the network.
            public void Forward(float [] inputsArray)
            {
                Nodes = new float [neurons];

                for(int i = 0;i < neurons ; i++)
                {
                    //sum of weights times inputs
                    for(int j = 0; j < inputs; j++)
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
                for(int i = 0; i < neurons; i++)
                {
                    for(int j = 0; j < inputs; j++)
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
        }
    }
}