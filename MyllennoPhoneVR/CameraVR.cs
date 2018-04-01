using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

/***************************************************
 *   Author: Lucas Myllenno S M Lima
 *   Data: 16/04/2016
 ***************************************************/

namespace MyllennoPhoneVR
{
    public class CameraVR
    {
        // Condições de rotação.
        private Boolean allow_RotationHorizontal;
        private Boolean allow_RotationVertical;
        private Boolean allow_RotationDiagonal;

        // Velocidades de movimentos da câmera.
        private float speed_Movement;
        private float speed_Rotation;

        // Versão da cabeça deitada.
        private Boolean mode_Lying;

        // Angulação inicial da bússola.
        private float orientation_horizontal_initial = 0.0000f;

        /* ------------------------------------------------------------------------------------------------
        /// Inicia a leitura dos sensores e o registro da leitura incial.
        /// ----------------------------------------------------------------------------------------------*/
        public CameraVR(Boolean allow_RotationHorizontal,
                        Boolean allow_RotationVertical,
                        Boolean allow_RotationDiagonal,
                        float speed_Rotation,
                        float speed_Movement,
                        Boolean mode_Lying)
        {
            this.allow_RotationHorizontal = allow_RotationHorizontal;
            this.allow_RotationVertical = allow_RotationVertical;
            this.allow_RotationDiagonal = allow_RotationDiagonal;

            this.speed_Movement = speed_Movement;
            this.speed_Rotation = speed_Rotation;

            this.mode_Lying = mode_Lying;

            Input.compass.enabled = true;
        }

        /* ------------------------------------------------------------------------------------------------
	/// Verifica se o dispositivo atual tem suporte aos sensores utilizados
	/// ----------------------------------------------------------------------------------------------*/
        public Boolean isSupported()
        {
            if (SystemInfo.supportsAccelerometer && Input.compass.enabled)
            {
                return true;
            }
            return false;
        }

        /* ------------------------------------------------------------------------------------------------
	/// A bússola retorna um valor de 0 a 360 graus que indica a posição Norte geograficamente.
	/// Condições:
	/// 	- A coordenada Y de rotação da câmera do jogo, será a ângulação atual da bússola 
	/// 	  retirando o valor dela ao iniciar o jogo. Ou seja, a orientação em que a câmera 
	/// 	  iniciou. Isso impede que o jogo inicie sempre com a frente para o Norte.
        /// ----------------------------------------------------------------------------------------------*/
        private float rotationHorizontal()
        {
            if (orientation_horizontal_initial == 0.0000f)
            {
                orientation_horizontal_initial = Input.compass.magneticHeading;
            }

            float horizontal = Input.compass.magneticHeading - orientation_horizontal_initial;

            if (horizontal < 0)
            {
                horizontal += 360;
            }

            return (int)(horizontal);
        }

        /* --------------------------------------------------------------------------------------------------
	/// A coordenada Z do acelerômetro retorna valores de -1 até +1 de acordo com a orientação 
	/// do smartphone deitado:
	/// Condições:
	/// 	- Esta coordenada multiplicado por (-180º) retorna o valor da inclinação diagonal 
	/// 	  da câmera, representado pela coordenada X.
        ///     - Se o modo "deitado" estiver ativado, a multiplicação passa a ser por (-90º).
        /// ----------------------------------------------------------------------------------------------- */
        private float rotationVertical()
        {
            float variable = 0.0f;

            if (mode_Lying == false)
            {
                variable = -180.0f;
            }
            else
            {
                variable = -90.0f;
            }

            float vertical = Input.acceleration.z * variable;

            return (int)(vertical);
        }

        /* --------------------------------------------------------------------------------------------------
	/// A coordenada X do acelerômetro retorna valores de -1 até +1 de acordo com a orientação 
	/// do smartphone deitado:
	/// Condições:
	/// 	- Esta coordenada multiplicado por (-180º) retorna o valor da inclinação diagonal 
	/// 	  da câmera, representado pela coordenada Z.
        ///     - Se o modo "deitado" estiver ativado, a multiplicação passa a ser por (-90º).
        /// ------------------------------------------------------------------------------------------------*/
        private float rotationDiagonal()
        {
            float variable = 0.0f;

            if (mode_Lying == false)
            {
                variable = -180.0f;
            }
            else
            {
                variable = -90.0f;
            }

            float diagonal = Input.acceleration.x * variable;

            return (int)(diagonal);
        }

        /* --------------------------------------------------------------------------------------------------
        /// Verifica os valores da inclinação horizontal, vertical e diagonal a partir dos sensores.
	/// Retorna o processamento destes valores para a inclinaçõa da câmera.
        /// ------------------------------------------------------------------------------------------------*/
        public Quaternion rotationCamera(Quaternion rotation_actual)
        {
            float horizontal = 0.0f;
            float vertical = 0.0f;
            float diagonal = 0.0f;

            if (allow_RotationHorizontal == true)
            {
                horizontal = rotationHorizontal();
            }
            if (allow_RotationVertical == true)
            {
                vertical = rotationVertical();
            }
            if (allow_RotationDiagonal == true)
            {
                diagonal = rotationDiagonal();
            }

            Quaternion rotation = Quaternion.Euler(vertical, horizontal, diagonal);

            Quaternion quaternion = Quaternion.Slerp(rotation_actual, rotation, Time.deltaTime * speed_Rotation);

            return quaternion;
        }

        /* -------------------------------------------------------------------------------------------------
        /// Retorna os valores X e Z de movimento das câmeras de visualização pelo cenário.
        /// -----------------------------------------------------------------------------------------------*/
        public Vector3 movementCamera(Vector3 position)
        {
            Vector3 movement = position * speed_Movement * Time.deltaTime;

            Vector3 movement_horizontal = new Vector3(movement.x, 0.0f, movement.z);

            return (movement_horizontal);
        }
    }
}
