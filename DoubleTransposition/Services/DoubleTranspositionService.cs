using DoubleTransposition.Interfaces;
using DoubleTransposition.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleTransposition.Services
{
    public class DoubleTranspositionService : IDoubleTranspositionService
    {
        public string Encrypt(DoubleTranspositionModel data)
        {
            string dataString = string.Empty;
            using (TextReader tr = new StreamReader(data.Stream))
                dataString = tr.ReadToEnd();

            char[] characters = PrepareText(dataString, data.RowKeys.Count, data.ColumnKeys.Count);

            string resultString = string.Empty;
            int offset = 0;
            int matrixSize = data.RowKeys.Count * data.ColumnKeys.Count;
            int numberOfMatrices = characters.Length / matrixSize;

            for (int i = 0; i < numberOfMatrices; i++)
            {
                char[,] dataMatrix = PrepareMatrix(characters, data.RowKeys.Count, data.ColumnKeys.Count, offset);

                dataMatrix = EncryptTransposeRows(dataMatrix, data.RowKeys.Count, data.ColumnKeys.Count, data.RowKeys);
                dataMatrix = EncryptTransposeColumns(dataMatrix, data.RowKeys.Count, data.ColumnKeys.Count, data.ColumnKeys);

                for (int j = 0; j < data.RowKeys.Count; j++)
                    for (int k = 0; k < data.ColumnKeys.Count; k++)
                        resultString += dataMatrix[j,k];

                offset +=  matrixSize;
            }

            return resultString;
        }

        public string Decrypt(DoubleTranspositionModel data)
        {
            string dataString = string.Empty;
            using (TextReader tr = new StreamReader(data.Stream))
                dataString = tr.ReadToEnd();

            char[] characters = dataString.ToCharArray();
            string resultString = string.Empty;
            int offset = 0;
            int matrixSize = data.RowKeys.Count * data.ColumnKeys.Count;
            int numberOfMatrices = characters.Length / matrixSize;

            for (int i = 0; i < numberOfMatrices; i++)
            {
                char[,] dataMatrix = PrepareMatrix(characters, data.RowKeys.Count, data.ColumnKeys.Count, offset);

                dataMatrix = DecryptTransposeColumns(dataMatrix, data.RowKeys.Count, data.ColumnKeys.Count, data.ColumnKeys);
                dataMatrix = DecryptTransposeRows(dataMatrix, data.RowKeys.Count, data.ColumnKeys.Count, data.RowKeys);

                for (int j = 0; j < data.RowKeys.Count; j++)
                    for (int k = 0; k < data.ColumnKeys.Count; k++)
                        resultString += dataMatrix[j, k];

                offset += matrixSize;
            }

            resultString = resultString.Replace(" ", ""); //remove padding which we added for encryption

            return resultString;
        }

        /// <summary>
        /// Removes special characters and numbers and turns everything into lower case letters. 
        /// Then pads the ending with white spaces, 
        /// so that we can divide data into matrixes the size of rowNumber*colNumber exactly.
        /// </summary>
        /// <param name="data">Data to clean up</param>
        /// <param name="rowNumber">Number of rows for permutation matrix</param>
        /// <param name="colNumber">Number of columns for permutation matrix</param>
        /// <returns>Clean char array padded with whitespaces</returns>
        public char[] PrepareText(string data, int rowNumber, int colNumber)
        {
            StringBuilder sb = new StringBuilder();
            int matrixSize = rowNumber * colNumber;
            foreach (char c in data)
            {
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }

            int missingCharsCount = matrixSize - sb.Length % matrixSize;
            for (int i = 0; i < missingCharsCount; i++)
                sb.Append(' ');

            string result = sb.ToString().ToLower();
            
            return result.ToCharArray();
        }
        /// <summary>
        /// Takes a chunk of the input data the size of a matrix (rowNumber*colNumber) and turns it into a matrix
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="rowNumber">Number of rows</param>
        /// <param name="colNumber">Number of columns</param>
        /// <param name="offset">Offset from which to start reading data</param>
        /// <returns>colNumber*rowNumber of characters in a matrix</returns>
        public char[,] PrepareMatrix(char[] data, int rowNumber, int colNumber, int offset)
        {
            char[,] matrix = new char[rowNumber, colNumber];

            for(int i = 0; i < rowNumber; i++)
                for(int j = 0; j < colNumber; j++)
                {
                    matrix[i,j] = data[offset + i * colNumber + j];
                }
            return matrix;
        }
        /// <summary>
        /// Takes the input matrix and transposes its rows according to the keys it recieves
        /// </summary>
        /// <param name="data">Matrix to be transposed</param>
        /// <param name="rowNumber">Number of rows of the input matrix</param>
        /// <param name="colNumber">Number of columns of the input matrix</param>
        /// <param name="transposingKey">List of integers which denote how the rows should be transposed</param>
        /// <returns>New transposed matrix</returns>
        public char[,] EncryptTransposeRows(char[,] data, int rowNumber, int colNumber, IList<int> transposingKey)
        {
            char[,] resultMatrix = new char[rowNumber, colNumber];

            for(int i = 0; i < rowNumber; i++)
                for(int j = 0; j < colNumber; j++)
                {
                    resultMatrix[i, j] = data[transposingKey[i], j];
                }
            return resultMatrix;
        }
        /// <summary>
        /// Takes the input matrix and transposes its columns according to the keys it recieves
        /// </summary>
        /// <param name="data">Matrix to be transposed</param>
        /// <param name="rowNumber">Number of rows of the input matrix</param>
        /// <param name="colNumber">Number of columns of the input matrix</param>
        /// <param name="transposingKey">List of integers which denote how the columns should be transposed</param>
        /// <returns>New transposed matrix</returns>
        public char[,] EncryptTransposeColumns(char[,] data, int rowNumber, int colNumber, IList<int> transposingKey)
        {
            char[,] resultMatrix = new char[rowNumber, colNumber];

            for (int j = 0; j < colNumber; j++)
                for (int i = 0; i < rowNumber; i++)
                {
                    resultMatrix[i, j] = data[i, transposingKey[j]];
                }
            return resultMatrix;

        }

        /// <summary>
        /// Takes the input matrix and transposes its rows according to the keys it recieves, 
        /// but in reverse order so to say, so that we can decrypt the data.
        /// </summary>
        /// <param name="data">Matrix to be transposed</param>
        /// <param name="rowNumber">Number of rows of the input matrix</param>
        /// <param name="colNumber">Number of columns of the input matrix</param>
        /// <param name="transposingKey">List of integers which denote how the rows should be transposed</param>
        /// <returns>New transposed matrix</returns>
        public char[,] DecryptTransposeRows(char[,] data, int rowNumber, int colNumber, IList<int> transposingKey)
        {
            char[,] resultMatrix = new char[rowNumber, colNumber];

            for (int i = 0; i < rowNumber; i++)
                for (int j = 0; j < colNumber; j++)
                {
                    resultMatrix[transposingKey[i], j] = data[i, j];
                }
            return resultMatrix;
        }
        /// <summary>
        /// Takes the input matrix and transposes its columns according to the keys it recieves, 
        /// but in reverse order so to say, so that we can decrypt the data.
        /// </summary>
        /// <param name="data">Matrix to be transposed</param>
        /// <param name="rowNumber">Number of rows of the input matrix</param>
        /// <param name="colNumber">Number of columns of the input matrix</param>
        /// <param name="transposingKey">List of integers which denote how the columns should be transposed</param>
        /// <returns>New transposed matrix</returns>
        public char[,] DecryptTransposeColumns(char[,] data, int rowNumber, int colNumber, IList<int> transposingKey)
        {
            char[,] resultMatrix = new char[rowNumber, colNumber];

            for (int j = 0; j < colNumber; j++)
                for (int i = 0; i < rowNumber; i++)
                {
                    resultMatrix[i, transposingKey[j]] = data[i, j];
                }
            return resultMatrix;

        }
    }
}
