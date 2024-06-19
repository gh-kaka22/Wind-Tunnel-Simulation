using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEditor.PackageManager;
using UnityEngine;
using Unity.Mathematics; 
using UnityEngine.Rendering;

public class LatticeBoltzmann_copy
{
    int xdim;
    int ydim;
    int zdim;

    // Cell Properties:
    public double[,,] n0, nN, nS, nE, nW, nF, nB, nNW, nNE, nSW, nSE, nBE, nBW, nBN, nBS, nFE, nFW, nFN, nFS;
    public double[,,] rho;
    public double[,,] ux, uy, uz;
    public double[,,] speed2;
    public bool[,,] barriers;
    public double[,,] curl;
    public double initSpeed = 0.10;

    public double velocity=1, viscosity=1;

    // Constants:
    double one3 = (double)1/3;
    double one18 = (double)1/18;
    double one36 = (double)1/36;

void zeroSite(int x, int y, int z)
    {
        n0[x, y, z] = 0;
        nE[x, y, z] = 0;
        nW[x, y, z] = 0;
        nN[x, y, z] = 0;
        nS[x, y, z] = 0;
        nB[x, y, z] = 0;
        nF[x, y, z] = 0;
        nNE[x, y, z] = 0;
        nNW[x, y, z] = 0;
        nSE[x, y, z] = 0;
        nSW[x, y, z] = 0;
        nBE[x, y, z] = 0;
        nBW[x, y, z] = 0;
        nBN[x, y, z] = 0;
        nBS[x, y, z] = 0;
        nFE[x, y, z] = 0;
        nFW[x, y, z] = 0;
        nFN[x, y, z] = 0;
        nFS[x, y, z] = 0;
        nE[x, y, z] = 0;
        nE[x, y, z] = 0;

        ux[x, y, z] = 0;
        uy[x, y, z] = 0;
        uz[x, y, z] = 0;

        speed2[x, y, z] = 0;
    }

    public void InitBarriers()
    {
        barriers  =  new bool[xdim, ydim, zdim];
        for(int x = 0; x < xdim; x ++){
            for(int y = 0; y < ydim; y ++){
                for(int z = 0 ; z < zdim; z++){
                    if(x == 2 && y==2 && z ==2)
                        barriers[x, y, z] = true;
                    else
                        barriers[x, y, z] = false;
                }   
            }
        }
        
    }
    
    public void setEqual(int x, int y,int z, int newux, int newuy,int newuz , int newrho)
    {
                        // double newrho = 1;
                        // double newux = 0, newuy = 0, newuz = this.velocity;
                        
                        double ux3 = 3 * newux;
                        double uy3 = 3 * newuy;
                        double uz3 = 3 * newuz;
                        double ux2 = newux * newux;
                        double uy2 = newuy * newuy;
                        double uz2 = newuz * newuz;
                        double uxuy2 = 2 * newux * newuy;
                        double uxuz2 = 2 * newux * newuz;
                        double uyuz2 = 2 * newuz * newuy;
                        double u2 = ux2 + uy2 + uz2;
                        double u215 = 1.5 * u2;
                        
                        
                        n0[x, y, z]  = one3 * newrho * (1                              - u215);
                        // UnityEngine.Debug.Log("n0000:" + (one3) );
                        nE[x, y, z]  =   one18 * newrho * (1 + ux3       + 4.5*ux2        - u215);
                        nW[x, y, z]  =   one18 * newrho * (1 - ux3       + 4.5*ux2        - u215);
                        nN[x, y, z]  =   one18 * newrho * (1 + uy3       + 4.5*uy2        - u215);
                        nS[x, y, z]  =   one18 * newrho * (1 - uy3       + 4.5*uy2        - u215);
                        nF[x, y, z]  =   one18 * newrho * (1 + uz3       + 4.5*uz2        - u215);//
                        nB[x, y, z]  =   one18 * newrho * (1 - uz3       + 4.5*uz2        - u215);//
                        nNE[x, y, z] =  one36 * newrho * (1 + ux3 + uy3 + 4.5*(u2+uxuy2) - u215);
                        nSE[x, y, z] =  one36 * newrho * (1 + ux3 - uy3 + 4.5*(u2-uxuy2) - u215);
                        nNW[x, y, z] =  one36 * newrho * (1 - ux3 + uy3 + 4.5*(u2-uxuy2) - u215);
                        nFE[x, y, z] =  one36 * newrho * (1 + ux3 + uz3 + 4.5*(u2+uxuz2) - u215);
                        nFW[x, y, z] =  one36 * newrho * (1 - ux3 + uz3 + 4.5*(u2-uxuz2) - u215);
                        nFN[x, y, z] =  one36 * newrho * (1 + uy3 + uz3 + 4.5*(u2+uyuz2) - u215);
                        nFS[x, y, z] =  one36 * newrho * (1 - uy3 + uz3 + 4.5*(u2-uyuz2) - u215);
                        nBE[x, y, z] =  one36 * newrho * (1 + ux3 - uz3 + 4.5*(u2-uxuz2) - u215);
                        nBW[x, y, z] =  one36 * newrho * (1 - ux3 - uz3 + 4.5*(u2+uxuz2) - u215);
                        nBN[x, y, z] =  one36 * newrho * (1 + uy3 - uz3 + 4.5*(u2-uyuz2) - u215);
                        nBS[x, y, z] =  one36 * newrho * (1 - uy3 - uz3 + 4.5*(u2+uyuz2) - u215);
		
                        rho[x, y, z] = newrho;
                        ux[x, y, z] = newux;
                        uy[x, y, z] = newuy;
                        uz[x, y, z] = newuz;
    }
    
// before calling this function: barriers should be intialized.
    public void InitFluid(int xdim, int ydim, int zdim)
    {
        // Init dimensions
        this.xdim = xdim;
        this.ydim = ydim;
        this.zdim = zdim;

        // Initialize barriers
        InitBarriers();

        // Initialize arrays
        n0 = new double[xdim, ydim, zdim];
        nE = new double[xdim, ydim, zdim];
        nW = new double[xdim, ydim, zdim];
        nN = new double[xdim, ydim, zdim];
        nS = new double[xdim, ydim, zdim];
        nB = new double[xdim, ydim, zdim];
        nF = new double[xdim, ydim, zdim];
        nNE = new double[xdim, ydim, zdim];
        nNW = new double[xdim, ydim, zdim];
        nSE = new double[xdim, ydim, zdim];
        nSW = new double[xdim, ydim, zdim];
        nBE = new double[xdim, ydim, zdim];
        nBW = new double[xdim, ydim, zdim];
        nBN = new double[xdim, ydim, zdim];
        nBS = new double[xdim, ydim, zdim];
        nFE = new double[xdim, ydim, zdim];
        nFW = new double[xdim, ydim, zdim];
        nFN = new double[xdim, ydim, zdim];
        nFS = new double[xdim, ydim, zdim];

        ux = new double[xdim, ydim, zdim];
        uy = new double[xdim, ydim, zdim];
        uz = new double[xdim, ydim, zdim];
        rho = new double[xdim, ydim, zdim];
        speed2 = new double[xdim, ydim, zdim]; 

        for (int x = 0; x < xdim; x++)
        {
            for (int y = 0; y < ydim; y++)
            {
                for (int z = 0; z < zdim; z++)
                {
                    int u0 = 1;
                    setEqual(x, y, z, 0, 0, u0, 1);
                }
            }
        }
    }


    

    public void Collide() 
    {
        double omega = 1 / (3*viscosity + 0.5);		// reciprocal of relaxation time
        
        for (int  y=1; y<ydim-1; y++) {
            for (int  x=1; x<xdim-1; x++) {
                for (int z = 1; z < zdim - 1; z++){
                    double thisrho = n0[x, y, z] + nN[x, y, z] + nS[x, y, z] + nE[x, y, z] + nF[x, y, z] + nB[x, y, z] +
                                     nW[x, y, z] + nNW[x, y, z] + nNE[x, y, z] + nSW[x, y, z] + nSE[x, y, z] + nFE[x, y, z] +
                                     nFW[x, y, z] + nFN[x, y, z] + nFS[x, y, z] + nBE[x, y, z] + nBW[x, y, z] + nBN[x, y, z] + nBS[x, y, z];
                    // if(barriers[x,y,z]) UnityEngine.Debug.Log("thisrhooooooooo" + thisrho);
                    
                    
                    
                    rho[x, y, z] = thisrho;
                    double thisux = (nE[x, y, z] + nNE[x, y, z] + nSE[x, y, z] - nW[x, y, z] - nNW[x, y, z] - nSW[x, y, z] + nFE[x, y, z] - nFW[x, y, z] + nBE[x, y, z] - nBW[x, y, z]) / thisrho;
                    ux[x, y, z] = thisux;
                    double thisuy = (nN[x, y, z] + nNE[x, y, z] + nNW[x, y, z] - nS[x, y, z] - nSE[x, y, z] - nSW[x, y, z] + nFN[x, y, z] - nFS[x, y, z] + nBN[x, y, z] - nBS[x, y, z]) / thisrho;
                    uy[x, y, z] = thisuy;
                    double thisuz = (nF[x, y, z] - nB[x, y, z] + nFE[x, y, z] + nFW[x, y, z] + nFN[x, y, z] + nFS[x, y, z] - nBE[x, y, z] - nBW[x, y, z] - nBN[x, y, z] - nBS[x, y, z]) / thisrho;
                    uz[x, y, z] = thisuz;
                    
                    double one3rho = one3 * thisrho;
                    double one18rho = one18 * thisrho;
                    double one36rho = one36 * thisrho;
                    
                    double ux3 = 3 * thisux;
                    double uy3 = 3 * thisuy;
                    double uz3 = 3 * thisuz;
                    double ux2 = thisux * thisux;
                    double uy2 = thisuy * thisuy;
                    double uz2 = thisuz * thisuz;
                    double uxuy2 = 2 * thisux * thisuy;
                    double uxuz2 = 2 * thisux * thisuz;
                    double uyuz2 = 2 * thisuz * thisuy;
                    double u2 = ux2 + uy2 + uz2;
                    double u215 = 1.5 * u2;
                    
                    
                    n0[x, y, z]  += omega * (one3rho * (1                               - u215) - n0[x, y, z]);
                    nE[x, y, z]  += omega * (one18rho * (1 + ux3       + 4.5*ux2        - u215) - nE[x, y, z]);
                    nW[x, y, z]  += omega * (one18rho * (1 - ux3       + 4.5*ux2        - u215) - nW[x, y, z]);
                    nN[x, y, z]  += omega * (one18rho * (1 + uy3       + 4.5*uy2        - u215) - nN[x, y, z]);
                    nS[x, y, z]  += omega * (one18rho * (1 - uy3       + 4.5*uy2        - u215) - nS[x, y, z]);
                    nF[x, y, z]  += omega * (one18rho * (1 + uz3       + 4.5*uz2        - u215) - nF[x, y, z]);
                    nB[x, y, z]  += omega * (one18rho * (1 - uz3       + 4.5*uz2        - u215) - nB[x, y, z]);
 
                    nNE[x, y, z] += omega * (one36rho * (1 + ux3 + uy3 + 4.5*(u2+uxuy2) - u215) - nNE[x, y, z]);
                    nSE[x, y, z] += omega * (one36rho * (1 + ux3 - uy3 + 4.5*(u2-uxuy2) - u215) - nSE[x, y, z]);
                    nNW[x, y, z] += omega * (one36rho * (1 - ux3 + uy3 + 4.5*(u2-uxuy2) - u215) - nNW[x, y, z]);
                    nSW[x, y, z] += omega * (one36rho * (1 - ux3 - uy3 + 4.5*(u2+uxuy2) - u215) - nSW[x, y, z]);
                    
                    nFE[x, y, z] += omega * (one36rho * (1 + ux3 + uz3 + 4.5*(u2+uxuz2) - u215) - nFE[x, y, z]);
                    nFW[x, y, z] += omega * (one36rho * (1 - ux3 + uz3 + 4.5*(u2-uxuz2) - u215) - nFW[x, y, z]);
                    nFN[x, y, z] += omega * (one36rho * (1 + uy3 + uz3 + 4.5*(u2+uyuz2) - u215) - nFN[x, y, z]);
                    nFS[x, y, z] += omega * (one36rho * (1 - uy3 + uz3 + 4.5*(u2-uyuz2) - u215) - nFS[x, y, z]);
                    nBE[x, y, z] += omega * (one36rho * (1 + ux3 - uz3 + 4.5*(u2-uxuz2) - u215) - nBE[x, y, z]);
                    nBW[x, y, z] += omega * (one36rho * (1 - ux3 - uz3 + 4.5*(u2+uxuz2) - u215) - nBW[x, y, z]);
                    nBN[x, y, z] += omega * (one36rho * (1 + uy3 - uz3 + 4.5*(u2-uyuz2) - u215) - nBN[x, y, z]);
                    nBS[x, y, z] += omega * (one36rho * (1 - uy3 - uz3 + 4.5*(u2+uyuz2) - u215) - nBS[x, y, z]);
                    // addition from me
                    speed2[x, y, z] = ux2 + uy2 + uz2;
                    // speed2[x,y,z] = ux[x,y,z]*ux[x,y,z] + uy[x,y,z]*uy[x,y,z]+ uz[x,y,z]*uz[x,y,z];
                    // double speed = math.sqrt(lattic.speed2[x, y, z]);
                    if(barriers[x,y,z])
                        UnityEngine.Debug.Log("Barierspeed:" + speed2[x, y, z]);
                    else
                    UnityEngine.Debug.Log("speed:" + speed2[x, y, z]);
                }
            }
            
        }
        
        for (int y = 1; y < ydim; y++)
        {
            for (int z = 1; z < zdim; z++)
            {
                nW[xdim - 1, y, z] = nW[xdim - 2, y, z];
                nNW[xdim - 1, y, z] = nNW[xdim - 2, y, z];
                nSW[xdim - 1, y, z] = nSW[xdim - 2, y, z];
                nFW[xdim - 1, y, z] = nFW[xdim - 2, y, z];
                nBW[xdim - 1, y, z] = nBW[xdim - 2, y, z];
            }
        }
        
    }


    public void Stream()
    {
        // FNE -x, -y, -z
        for(int x = xdim-2; x>0; x--){
            for(int y = ydim-2; y>0; y--){
                for(int z = zdim-2; z>0; z--){
                    // F, E, FE, EN, FN
                    nF[x, y, z] = nF[x, y, z-1];

                    nFE[x, y, z] = nFE[x-1, y, z-1];
                    nNE[x, y, z] = nNE[x-1, y-1, z];
                    nFN[x, y, z] = nFN[x, y-1, z-1];
                }
            }
        }

        // FSW x, y, -z 
        for(int x = 1; x<xdim-1; x++){
            for(int y = 1; y<ydim-1; y++){
                for(int z = zdim-2; z>0; z--){
                    // W, FW, FS, WS
                    nW[x, y, z] = nW[x+1, y, z];
                    nS[x, y, z] = nS[x, y+1, z];
                    

                    nFW[x, y, z] = nFW[x+1, y, z-1];
                    nFS[x, y, z] = nFS[x, y+1, z-1];
                    nSW[x, y, z] = nSW[x+1, y+1, z];
                }
            }
        }
 
        // BSE -x, y, z
        for(int x = xdim-2; x>0; x--){
            for(int y = 1; y<ydim-1; y++){
                for(int z = 1; z<zdim-1;z++){
                    // B, S, BE, BS, ES
                    nB[x, y, z] = nB[x, y, z+1];
                    nE[x, y, z] = nE[x-1, y, z];

                    nBE[x, y, z] = nBE[x-1, y, z+1];
                    nBS[x, y, z] = nBS[x, y+1, z+1];
                    nSE[x, y, z] = nSE[x-1, y+1, z];
                }
            }
        }

        // BNW x, -y, z
        for(int x = 1; x<xdim-1; x++){
            for(int y = ydim-2; y>0; y--){
                for(int z = 1; z<zdim-1; z++){
                    // N, BN, WN, BW
                    nN[x, y, z] = nN[x, y-1, z];

                    nBN[x, y, z] = nBN[x, y-1, z+1];
                    nNW[x, y, z] = nNW[x+1, y-1, z];
                    nBW[x, y, z] = nBW[x+1, y, z+1];
                }
            }
        }
 
        // Bouncing of Barriers
        for(int x = 1; x < xdim-1; x++){
            for(int y = 1; y < ydim-1; y++){
                for(int z = 1; z < zdim-1; z++){
                    if(barriers[x, y, z]){
                        
                        nE[x+1, y, z] = nW[x, y, z];
                        nW[x-1, y, z] = nE[x, y, z];
                        nN[x, y+1, z] = nS[x, y, z];
                        nS[x, y-1, z] = nN[x, y, z];
                        nF[x, y, z+1] = nB[x, y, z];
                        nB[x, y, z-1] = nF[x, y, z];

                        nNE[x+1, y+1, z] = nSW[x, y, z];
                        nNW[x-1, y+1, z] = nSE[x, y, z];
                        nSE[x+1, y-1, z] = nNW[x, y, z];
                        nSW[x-1, y-1, z] = nNE[x, y, z];

                        nFN[x, y+1, z+1] = nBS[x, y, z];
                        nFS[x, y-1, z+1] = nBN[x, y, z];
                        nFE[x+1, y, z+1] = nBW[x, y, z];
                        nFW[x-1, y, z+1] = nBE[x, y, z];

                        nBN[x, y+1, z-1] = nFS[x, y, z];
                        nBS[x, y-1, z-1] = nFN[x, y, z];
                        nBE[x+1, y, z-1] = nFW[x, y, z];
                        nBW[x-1, y, z-1] = nFE[x, y, z];
                    }
                }
            }
        }
    }

    

    // // Start is called before the first frame update
    // void Start()
    // {
    //     // InitFluid();
    // }



    // // Update is called once per frame
    // void Update()
    // {
    //     // Collide();
    //     // Stream();
    // }
}
