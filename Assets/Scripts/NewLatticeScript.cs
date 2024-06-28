using System;
using UnityEngine;
using Unity.Mathematics;

public class NewLatticeScript : MonoBehaviour
{
    /*
     * اعتبر حجم المصفوفة 
xdim,ydim,zdim
ydim*zdim*x+zdim*y+z
    */
    public double[,,] n0, nN, nS, nE, nW, nF, nB, nNW, nNE, nSW, nSE, nBE, nBW, nBN, nBS, nFE, nFW, nFN, nFS;
    public double[,,] rho;
    public double[,,] ux, uy, uz;
    public double[,,] speed2;
    // public double[,,] u2;
    public bool[,,] barriers;
    public double[,,] curl;
    public double initSpeed = 0.1, initViscosity = 0.02;
    public double max_speed = -100000.0, min_speed = 100000.0;
    public double max_curl = -100000.0, min_curl = 100000.0;
    public double fx, fy, fz;


    private bool running = false;
    // public double velocity=1, viscosity=1;

    // double one3 = (double)1/3;
    // double one18 = (double)1/18;
    // double one36 = (double)1/36;
    double one3 = Math.Round((double)1 / 3, 6);
    double one18 = Math.Round((double)1 / 18, 6);
    double one36 = Math.Round((double)1 / 36, 6);
    //
    private int xdim, ydim, zdim;

    public void SetSize(int xdim, int ydim, int zdim, int mxdim, int mydim, int mzdim)
    {
        this.xdim = xdim;
        this.ydim = ydim;
        this.zdim = zdim;

        InitializeArrays(mxdim, mydim, mzdim);
    }

    public void InitializeArrays(int mxdim, int mydim, int mzdim)
    {
        n0 = new double[mxdim, mydim, mzdim];
        nN = new double[mxdim, mydim, mzdim];
        nS = new double[mxdim, mydim, mzdim];
        nE = new double[mxdim, mydim, mzdim];
        nW = new double[mxdim, mydim, mzdim];
        nF = new double[mxdim, mydim, mzdim];
        nB = new double[mxdim, mydim, mzdim];
        nNW = new double[mxdim, mydim, mzdim];
        nNE = new double[mxdim, mydim, mzdim];
        nSW = new double[mxdim, mydim, mzdim];
        nSE = new double[mxdim, mydim, mzdim];
        nBE = new double[mxdim, mydim, mzdim];
        nBW = new double[mxdim, mydim, mzdim];
        nBN = new double[mxdim, mydim, mzdim];
        nBS = new double[mxdim, mydim, mzdim];
        nFE = new double[mxdim, mydim, mzdim];
        nFW = new double[mxdim, mydim, mzdim];
        nFN = new double[mxdim, mydim, mzdim];
        nFS = new double[mxdim, mydim, mzdim];
        rho = new double[mxdim, mydim, mzdim];
        ux = new double[mxdim, mydim, mzdim];
        uy = new double[mxdim, mydim, mzdim];
        uz = new double[mxdim, mydim, mzdim];
        speed2 = new double[mxdim, mydim, mzdim];
        // u2 = new double[xdim, ydim, zdim];
        barriers = new bool[mxdim, mydim, mzdim];
        curl = new double[mxdim, mydim, mzdim];
    }


    public void InitBarriers(bool[,,] externalBarriers)
    {
        barriers = externalBarriers;
    }

    public void initFluid(int xdim, int ydim, int zdim)
    {
        double speed = this.initSpeed;

        for (int x = 0; x < xdim; x++)
        {
            for (int y = 0; y < ydim; y++)
            {
                for (int z = 0; z < zdim; z++)
                {
                    setEqual(x, y, z, speed, 0, 0, 1f);
                    curl[x, y, z] = 0.0;
                    double speedd = math.sqrt(speed2[x, y, z]);
                    max_speed = math.max(max_speed, speedd);
                    min_speed = math.min(min_speed, speedd);
                    // if (barriers[x, y, z])
                    //     zeroSite(x,y,z);
                }
            }
        }
    }
    public void zeroSite(int x, int y, int z)
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
    public void InitWithBarriers(int xdim, int ydim, int zdim, bool[,,] externalBarriers, int mxdim, int mydim, int mzdim)
    {
        SetSize(xdim, ydim, zdim, mxdim, mydim, mzdim);
        // Debug.Log($"Barrier at (2,2,2) before passing: {barriers[2, 2, 2]}");

        barriers = externalBarriers;  // Use the passed barriers
        // Debug.Log($"Barrier at (2,2,2) after receiving: {barriers[2, 2, 2]}");
        // InitBarriers();  // Initialize or adjust barriers as needed
        initFluid(xdim, ydim, zdim);  // Initialize fluid with barriers in place
    }

    void computeCurl()
    {
        for (int y = 1; y < ydim - 1; y++)
        {           // interior sites only; leave edges set to zero
            for (int x = 1; x < xdim - 1; x++)
            {
                for (int z = 1; z < zdim - 1; z++)
                {
                    curl[x, y, z] = uy[x + 1, y, z] - uy[x - 1, y, z] - (ux[x, y + 1, z] - uy[x, y - 1, z]);

                    double cur = curl[x, y, z];
                    max_curl = math.max(max_curl, cur);
                    min_curl = math.min(min_curl, cur);
                }
            }
        }
    }

    public void startStop()
    {
        running = !running;
        if (running)
        {
            // startButton.value = "Pause";
            // resetTimer();
            // simulate();
        }
        else
        {
            // startButton.value = " Run ";
        }
    }

    public void setEqual(int x, int y, int z, double newux, double newuy, double newuz, double newrho)
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
        // Debug.Log(u2);
        double u215 = 1.5 * u2;
        speed2[x, y, z] = ux2 + uy2 + uz2;
        // Debug.Log("speed2   " + speed2[x,y,z]);


        n0[x, y, z] = one3 * newrho * (1 - u215);
        // UnityEngine.Debug.Log("n0000:" + (one3) );
        nE[x, y, z] = one18 * newrho * (1 + ux3 + 4.5 * ux2 - u215);
        nW[x, y, z] = one18 * newrho * (1 - ux3 + 4.5 * ux2 - u215);
        nN[x, y, z] = one18 * newrho * (1 + uy3 + 4.5 * uy2 - u215);
        nS[x, y, z] = one18 * newrho * (1 - uy3 + 4.5 * uy2 - u215);
        nF[x, y, z] = one18 * newrho * (1 + uz3 + 4.5 * uz2 - u215);
        nB[x, y, z] = one18 * newrho * (1 - uz3 + 4.5 * uz2 - u215);
        nNE[x, y, z] = one36 * newrho * (1 + ux3 + uy3 + 4.5 * (u2 + uxuy2) - u215);
        nSE[x, y, z] = one36 * newrho * (1 + ux3 - uy3 + 4.5 * (u2 - uxuy2) - u215);
        nSW[x, y, z] = one36 * newrho * (1 - ux3 - uy3 + 4.5 * (u2 + uxuy2) - u215);
        nNW[x, y, z] = one36 * newrho * (1 - ux3 + uy3 + 4.5 * (u2 - uxuy2) - u215);
        nFE[x, y, z] = one36 * newrho * (1 + ux3 + uz3 + 4.5 * (u2 + uxuz2) - u215);
        nFW[x, y, z] = one36 * newrho * (1 - ux3 + uz3 + 4.5 * (u2 - uxuz2) - u215);
        nFN[x, y, z] = one36 * newrho * (1 + uy3 + uz3 + 4.5 * (u2 + uyuz2) - u215);
        nFS[x, y, z] = one36 * newrho * (1 - uy3 + uz3 + 4.5 * (u2 - uyuz2) - u215);
        nBE[x, y, z] = one36 * newrho * (1 + ux3 - uz3 + 4.5 * (u2 - uxuz2) - u215);
        nBW[x, y, z] = one36 * newrho * (1 - ux3 - uz3 + 4.5 * (u2 + uxuz2) - u215);
        nBN[x, y, z] = one36 * newrho * (1 + uy3 - uz3 + 4.5 * (u2 - uyuz2) - u215);
        nBS[x, y, z] = one36 * newrho * (1 - uy3 - uz3 + 4.5 * (u2 + uyuz2) - u215);

        rho[x, y, z] = newrho;
        ux[x, y, z] = newux;
        uy[x, y, z] = newuy;
        uz[x, y, z] = newuz;
    }

    public void simulate()
    {
        // things to execute for every frame
        // setBoundaries();
        collide();
        stream();
    }

    public void setBoundaries()
    {
        double speed = this.initSpeed;


        for (int z = 0; z < zdim; z++)
        {
            for (int x = 0; x < xdim; x++)
            {
                setEqual(x, 0, z, speed, 0, 0, 1); // Ground (y=0)
                setEqual(x, ydim - 1, z, speed, 0, 0, 1); // ceil(y=ydim-1)
            }
        }
        for (int z = 0; z < zdim; z++)
        {
            for (int y = 1; y < ydim - 1; y++)
            {
                setEqual(0, y, z, speed, 0, 0, 1); // Left face (x=0)
                setEqual(xdim - 1, y, z, speed, 0, 0, 1); // Right face (x=xdim-1)
            }
        }
        for (int x = 1; x < xdim - 1; x++)
        {
            for (int y = 1; y < ydim - 1; y++)
            {
                setEqual(x, y, 0, speed, 0, 0, 1); // back face (z=0)
                setEqual(x, y, zdim - 1, speed, 0, 0, 1); // front face (z=zdim-1)
            }
        }
    }
    public void collide()
    {
        double viscosity = this.initViscosity;
        double omega = 1 / (3 * viscosity + 0.5);       // reciprocal of relaxation time

        for (int y = 1; y < ydim - 1; y++)
        {
            for (int x = 1; x < xdim - 1; x++)
            {
                for (int z = 1; z < zdim - 1; z++)
                {
                    double thisrho = n0[x, y, z] + nN[x, y, z] + nS[x, y, z] + nE[x, y, z] + nF[x, y, z] + nB[x, y, z] +
                                     nW[x, y, z] + nNW[x, y, z] + nNE[x, y, z] + nSW[x, y, z] + nSE[x, y, z] + nFE[x, y, z] +
                                     nFW[x, y, z] + nFN[x, y, z] + nFS[x, y, z] + nBE[x, y, z] + nBW[x, y, z] + nBN[x, y, z] + nBS[x, y, z];
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

                    n0[x, y, z] += omega * (one3rho * (1 - u215) - n0[x, y, z]);
                    nE[x, y, z] += omega * (one18rho * (1 + ux3 + 4.5 * ux2 - u215) - nE[x, y, z]);
                    nW[x, y, z] += omega * (one18rho * (1 - ux3 + 4.5 * ux2 - u215) - nW[x, y, z]);
                    nN[x, y, z] += omega * (one18rho * (1 + uy3 + 4.5 * uy2 - u215) - nN[x, y, z]);
                    nS[x, y, z] += omega * (one18rho * (1 - uy3 + 4.5 * uy2 - u215) - nS[x, y, z]);
                    nF[x, y, z] += omega * (one18rho * (1 + uz3 + 4.5 * uz2 - u215) - nF[x, y, z]);
                    nB[x, y, z] += omega * (one18rho * (1 - uz3 + 4.5 * uz2 - u215) - nB[x, y, z]);

                    nNE[x, y, z] += omega * (one36rho * (1 + ux3 + uy3 + 4.5 * (u2 + uxuy2) - u215) - nNE[x, y, z]);
                    nSE[x, y, z] += omega * (one36rho * (1 + ux3 - uy3 + 4.5 * (u2 - uxuy2) - u215) - nSE[x, y, z]);
                    nNW[x, y, z] += omega * (one36rho * (1 - ux3 + uy3 + 4.5 * (u2 - uxuy2) - u215) - nNW[x, y, z]);
                    nSW[x, y, z] += omega * (one36rho * (1 - ux3 - uy3 + 4.5 * (u2 + uxuy2) - u215) - nSW[x, y, z]);

                    nFE[x, y, z] += omega * (one36rho * (1 + ux3 + uz3 + 4.5 * (u2 + uxuz2) - u215) - nFE[x, y, z]);
                    nFW[x, y, z] += omega * (one36rho * (1 - ux3 + uz3 + 4.5 * (u2 - uxuz2) - u215) - nFW[x, y, z]);
                    nFN[x, y, z] += omega * (one36rho * (1 + uy3 + uz3 + 4.5 * (u2 + uyuz2) - u215) - nFN[x, y, z]);
                    nFS[x, y, z] += omega * (one36rho * (1 - uy3 + uz3 + 4.5 * (u2 - uyuz2) - u215) - nFS[x, y, z]);
                    nBE[x, y, z] += omega * (one36rho * (1 + ux3 - uz3 + 4.5 * (u2 - uxuz2) - u215) - nBE[x, y, z]);
                    nBW[x, y, z] += omega * (one36rho * (1 - ux3 - uz3 + 4.5 * (u2 + uxuz2) - u215) - nBW[x, y, z]);
                    nBN[x, y, z] += omega * (one36rho * (1 + uy3 - uz3 + 4.5 * (u2 - uyuz2) - u215) - nBN[x, y, z]);
                    nBS[x, y, z] += omega * (one36rho * (1 - uy3 - uz3 + 4.5 * (u2 + uyuz2) - u215) - nBS[x, y, z]);
                    speed2[x, y, z] = ux2 + uy2 + uz2;

                    n0[x, y, z] = Math.Round(n0[x, y, z], 7);
                    nE[x, y, z] = Math.Round(nE[x, y, z], 7);
                    nW[x, y, z] = Math.Round(nW[x, y, z], 7);
                    nN[x, y, z] = Math.Round(nN[x, y, z], 7);
                    nS[x, y, z] = Math.Round(nS[x, y, z], 7);
                    nF[x, y, z] = Math.Round(nF[x, y, z], 7);
                    nB[x, y, z] = Math.Round(nB[x, y, z], 7);
                    nNE[x, y, z] = Math.Round(nNE[x, y, z], 7);
                    nSE[x, y, z] = Math.Round(nSE[x, y, z], 7);
                    nNW[x, y, z] = Math.Round(nNW[x, y, z], 7);
                    nSW[x, y, z] = Math.Round(nSW[x, y, z], 7);
                    nFE[x, y, z] = Math.Round(nFE[x, y, z], 7);
                    nFW[x, y, z] = Math.Round(nFW[x, y, z], 7);
                    nFN[x, y, z] = Math.Round(nFN[x, y, z], 7);
                    nFS[x, y, z] = Math.Round(nFS[x, y, z], 7);
                    nBE[x, y, z] = Math.Round(nBE[x, y, z], 7);
                    nBW[x, y, z] = Math.Round(nBW[x, y, z], 7);
                    nBN[x, y, z] = Math.Round(nBN[x, y, z], 7);
                    nBS[x, y, z] = Math.Round(nBS[x, y, z], 7);
                    speed2[x, y, z] = Math.Round(speed2[x, y, z], 7);

                    // double speed = math.sqrt(speed2[x, y, z]);
                    //     
                    // float speedf = (float)speed  ;
                    // Debug.Log($"Barrier at (2,2,2) in collide: {barriers[2, 2, 2]}");
                    // if(barriers[x, y, z])
                    //     Debug.Log("barSpeed  "+speedf);
                    // else 
                    //     Debug.Log("defSpeed  "+speedf);
                    double speedd = math.sqrt(speed2[x, y, z]);
                    max_speed = math.max(max_speed, speedd);
                    min_speed = math.min(min_speed, speedd);
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
        computeCurl();
    }

    public void stream()
    {
        // FNE -x, -y, -z
        fx = 0.0; fy = 0.0; fz = 0.0;

        for (int x = xdim - 2; x > 0; x--)
        {
            for (int y = ydim - 2; y > 0; y--)
            {
                for (int z = zdim - 2; z > 0; z--)
                {
                    // N, F, FE, EN, FN
                    // if(barriers[x,y,z])
                    //     continue;
                    nN[x, y, z] = nN[x, y - 1, z];
                    nF[x, y, z] = nF[x, y, z - 1];

                    nFE[x, y, z] = nFE[x - 1, y, z - 1];
                    nNE[x, y, z] = nNE[x - 1, y - 1, z];
                    nFN[x, y, z] = nFN[x, y - 1, z - 1];
                }
            }
        }


        // BSE -x, y, z
        for (int x = xdim - 2; x > 0; x--)
        {
            for (int y = 1; y < ydim - 1; y++)
            {
                for (int z = 1; z < zdim - 1; z++)
                {
                    // B, E, BE, BS, ES
                    // if(barriers[x,y,z])
                    //     continue;
                    nB[x, y, z] = nB[x, y, z + 1];
                    nE[x, y, z] = nE[x - 1, y, z];

                    nBE[x, y, z] = nBE[x - 1, y, z + 1];
                    nBS[x, y, z] = nBS[x, y + 1, z + 1];
                    nSE[x, y, z] = nSE[x - 1, y + 1, z];
                }
            }
        }

        // FSW x, y, -z 
        for (int x = 1; x < xdim - 1; x++)
        {
            for (int y = 1; y < ydim - 1; y++)
            {
                for (int z = zdim - 2; z > 0; z--)
                {
                    // S, FW, FS, WS
                    // if(barriers[x,y,z])
                    //     continue;
                    nS[x, y, z] = nS[x, y + 1, z];


                    nFW[x, y, z] = nFW[x + 1, y, z - 1];
                    nFS[x, y, z] = nFS[x, y + 1, z - 1];
                    nSW[x, y, z] = nSW[x + 1, y + 1, z];
                }
            }
        }

        // BNW x, -y, z
        for (int x = 1; x < xdim - 1; x++)
        {
            for (int y = ydim - 2; y > 0; y--)
            {
                for (int z = 1; z < zdim - 1; z++)
                {
                    // W, BN, WN, BW
                    // if(barriers[x,y,z])
                    //     continue;
                    nW[x, y, z] = nW[x + 1, y, z];

                    nBN[x, y, z] = nBN[x, y - 1, z + 1];
                    nNW[x, y, z] = nNW[x + 1, y - 1, z];
                    nBW[x, y, z] = nBW[x + 1, y, z + 1];
                }
            }
        }


        // Bouncing of Barriers

        for (int x = 1; x < xdim - 1; x++)
        {
            for (int y = 1; y < ydim - 1; y++)
            {
                for (int z = 1; z < zdim - 1; z++)
                {
                    if (barriers[x, y, z])
                    {

                        nE[x + 1, y, z] = nW[x, y, z];
                        nW[x - 1, y, z] = nE[x, y, z];
                        nN[x, y + 1, z] = nS[x, y, z];
                        nS[x, y - 1, z] = nN[x, y, z];
                        nF[x, y, z + 1] = nB[x, y, z];
                        nB[x, y, z - 1] = nF[x, y, z];

                        nNE[x + 1, y + 1, z] = nSW[x, y, z];
                        nNW[x - 1, y + 1, z] = nSE[x, y, z];
                        nSE[x + 1, y - 1, z] = nNW[x, y, z];
                        nSW[x - 1, y - 1, z] = nNE[x, y, z];

                        nFN[x, y + 1, z + 1] = nBS[x, y, z];
                        nFS[x, y - 1, z + 1] = nBN[x, y, z];
                        nFE[x + 1, y, z + 1] = nBW[x, y, z];
                        nFW[x - 1, y, z + 1] = nBE[x, y, z];

                        nBN[x, y + 1, z - 1] = nFS[x, y, z];
                        nBS[x, y - 1, z - 1] = nFN[x, y, z];
                        nBE[x + 1, y, z - 1] = nFW[x, y, z];
                        nBW[x - 1, y, z - 1] = nFE[x, y, z];


                        // calculating force:
                        fx += (nE[x, y, z] + nNE[x, y, z] + nSE[x, y, z] - nW[x, y, z] - nNW[x, y, z] - nSW[x, y, z] + nFE[x, y, z] - nFW[x, y, z] + nBE[x, y, z] - nBW[x, y, z]);
                        fy += (nN[x, y, z] + nNE[x, y, z] + nNW[x, y, z] - nS[x, y, z] - nSE[x, y, z] - nSW[x, y, z] + nFN[x, y, z] - nFS[x, y, z] + nBN[x, y, z] - nBS[x, y, z]);
                        fz += (nF[x, y, z] - nB[x, y, z] + nFE[x, y, z] + nFW[x, y, z] + nFN[x, y, z] + nFS[x, y, z] - nBE[x, y, z] - nBW[x, y, z] - nBN[x, y, z] - nBS[x, y, z]);
                    }
                }
            }
        }

        // handleSolidWalls();
    }

    private void handleSolidWalls()
    {
        // East and West walls
        for (int y = 1; y < ydim - 1; y++)
        {
            for (int z = 1; z < zdim - 1; z++)
            {
                // East wall
                nE[xdim - 1, y, z] = nW[xdim - 2, y, z];
                // West wall
                nW[0, y, z] = nE[1, y, z];
            }
        }

        // North and South walls
        for (int x = 1; x < xdim - 1; x++)
        {
            for (int z = 1; z < zdim - 1; z++)
            {
                // North wall
                nN[x, ydim - 1, z] = nS[x, ydim - 2, z];
                // South wall
                nS[x, 0, z] = nN[x, 1, z];
            }
        }

        // Top and Bottom walls (Front and Back in some contexts)
        for (int x = 1; x < xdim - 1; x++)
        {
            for (int y = 1; y < ydim - 1; y++)
            {
                // Top wall (Front)
                nF[x, y, zdim - 1] = nB[x, y, zdim - 2];
                // Bottom wall (Back)
                nB[x, y, 0] = nF[x, y, 1];
            }
        }
    }



}





// for(int x = 1; x < xdim-1; x++){
//     for(int y = 1; y < ydim-1; y++){
//         for(int z = 1; z < zdim-1; z++){
//             if(barriers[x, y, z]){
//                 
//                 if(!barriers[x+1,y,z])nE[x+1, y, z] = nW[x, y, z];
//                 if(!barriers[x-1,y,z])nW[x-1, y, z] = nE[x, y, z];
//                 if(!barriers[x,y+1,z])nN[x, y+1, z] = nS[x, y, z];
//                 if(!barriers[x,y-1,z])nS[x, y-1, z] = nN[x, y, z];
//                 if(!barriers[x,y,z+1])nF[x, y, z+1] = nB[x, y, z];
//                 if(!barriers[x,y,z-1])nB[x, y, z-1] = nF[x, y, z];
//
//                 if(!barriers[x+1,y+1,z])nNE[x+1, y+1, z] = nSW[x, y, z];
//                 if(!barriers[x-1,y+1,z])nNW[x-1, y+1, z] = nSE[x, y, z];
//                 if(!barriers[x+1,y-1,z])nSE[x+1, y-1, z] = nNW[x, y, z];
//                 if(!barriers[x-1,y-1,z])nSW[x-1, y-1, z] = nNE[x, y, z];
//
//                 if(!barriers[x,y+1,z+1])nFN[x, y+1, z+1] = nBS[x, y, z];
//                 if(!barriers[x,y-1,z+1])nFS[x, y-1, z+1] = nBN[x, y, z];
//                 if(!barriers[x+1,y,z+1])nFE[x+1, y, z+1] = nBW[x, y, z];
//                 if(!barriers[x-1,y,z+1])nFW[x-1, y, z+1] = nBE[x, y, z];
//
//                 if(!barriers[x,y+1,z-1])nBN[x, y+1, z-1] = nFS[x, y, z];
//                 if(!barriers[x,y-1,z-1])nBS[x, y-1, z-1] = nFN[x, y, z];
//                 if(!barriers[x+1,y,z-1])nBE[x+1, y, z-1] = nFW[x, y, z];
//                 if(!barriers[x-1,y,z-1])nBW[x-1, y, z-1] = nFE[x, y, z];
//             }
//         }
//     }
// }



/*
 * for (int y = 1; y < ydim; y++)
    {
        for (int x = 1; x < xdim; x++)
        {
            nW[zdim - 1, y, x]  =  nW[zdim - 2, y, x];
            nNW[zdim - 1, y, x] = nNW[zdim - 2, y, x];
            nSW[zdim - 1, y, x] = nSW[zdim - 2, y, x];
            nFW[zdim - 1, y, x] = nFW[zdim - 2, y, x];
            nBW[zdim - 1, y, x] = nBW[zdim - 2, y, x];
        }
    }
}
 */


// for (int y = 1; y < ydim; y++)
// {
//     for (int z = 1; z < zdim; z++)
//     {
//         nW[xdim - 1, y, z] = 0;
//         nNW[xdim - 1, y, z] = 0;
//         nSW[xdim - 1, y, z] = 0;
//         nFW[xdim - 1, y, z] = 0;
//         nBW[xdim - 1, y, z] = 0;
//     }
// }