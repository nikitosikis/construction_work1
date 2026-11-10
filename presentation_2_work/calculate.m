function calculate()
% Educational calculation. SI units except shaft speed (rpm) and reported micrometres.
g = struct('D',.030,'L',.036,'c',15e-6,'rpm',40000,'ps',600000, ...
    'pa',101325,'T',293.15,'mu',1.81e-5,'Rg',287.05,'gamma',1.4, ...
    'd',.16e-3,'Cd',.70,'spread',.46e-3,'rows',2,'holes',8, ...
    'S',.090,'a',.025,'m',.55,'Fr',25,'Fa',10,'ca',12e-6, ...
    'ri',.016,'rg',.020,'ro',.025,'da',.30e-3);
W=g.m*9.81;
RA=g.Fr*(1+g.a/g.S)+W/2;
RB=-g.Fr*g.a/g.S+W/2;
coarse=grid(g,96,36);
fine=grid(g,192,72);
cx=radialEquilibrium(g,coarse,RA,[0;0]);
front=radialEquilibrium(g,fine,RA,cx.offset(:)*1e-6);
rear=radialEquilibrium(g,fine,RB,[0;0]);
rest=radial(g,fine,[0;0]);
assert(norm(rest.F)<1e-7,'Concentric bearing reaction is not zero');
step=0.10e-6;
K=-(radialForceMatrix(g,fine,front.offset(:)*1e-6,step))*1e-6;
assert(all(diag(K)>0),'Static direct stiffness must be restoring');
g0=g; g0.rpm=0;
stationary=radialEquilibrium(g0,fine,RA,front.offset(:)*1e-6);
assert(abs(stationary.offset(2))<0.02,'No cross displacement expected at rest');
xy=(front.offset-rear.offset)*1e-6;
slope=norm(xy)/g.S;
front.tiltBound=front.hmin - 1e6*slope*g.L/2;
rear.tiltBound=rear.hmin - 1e6*slope*g.L/2;
tip=front.offset+(front.offset-rear.offset)*g.a/g.S;
load=0:10:60;
hvals=zeros(size(load));
off=[0;0];
for k=1:numel(load)
    rr=radialEquilibrium(g,fine,load(k),off);
    hvals(k)=rr.hmin; off=rr.offset(:)*1e-6;
end
assert(all(diff(hvals)<0),'Minimum gap must drop along this load sweep');

az=fzero(@(s) axial(g,g.ca-s).F-axial(g,g.ca+s).F-g.Fa,[0,.75*g.ca]);
ap=axial(g,g.ca-az); am=axial(g,g.ca+az);
kz=((axial(g,g.ca-az-step).F-axial(g,g.ca+az+step).F) ...
    -(axial(g,g.ca-az+step).F-axial(g,g.ca+az-step).F))/(2*step)*1e-6;
assert(abs(ap.F-am.F-g.Fa)<1e-5,'Axial equilibrium not met');
assert(kz>0,'Axial stiffness is not restoring');
qax=ap.Q+am.Q;
qa=struct('coarseFront',cx,'fineFrontGapChangeUm',abs(cx.hmin-front.hmin), ...
    'restResultant',norm(rest.F),'forceBalanceN',norm(front.reaction(:)+[RA;0]), ...
    'momentBalanceNm',abs(RB*g.S-W*g.S/2+g.Fr*g.a), ...
    'rotationOff',stationary,'relativeFlowImbalance',max([front.massError,rear.massError]));
assert(qa.fineFrontGapChangeUm < 0.5,'Gap is not mesh converged sufficiently');
assert(qa.relativeFlowImbalance<1e-6,'Mass imbalance');
out=struct('input',g,'speed',pi*g.D*g.rpm/60,'weight',W,'RA',RA,'RB',RB, ...
    'front',front,'rear',rear,'Kr',K,'slope',slope,'tipOffset',tip, ...
    'sweep',struct('load',load,'minGap',hvals), ...
    'axial',struct('offset',az*1e6,'gap1',(g.ca-az)*1e6,'gap2',(g.ca+az)*1e6, ...
    'F1',ap.F,'F2',am.F,'p1kPa',ap.pg/1000,'p2kPa',am.pg/1000,'Q',qax,'K',kz), ...
    'totalQ',front.Q+rear.Q+qax,'qa',qa);
f=fopen(fullfile(fileparts(mfilename('fullpath')),'results.json'),'w','n','UTF-8');
assert(f>=0,'Cannot open result file');
fprintf(f,'%s',jsonencode(out,'PrettyPrint',true)); fclose(f);
disp(out);
end

function gg=grid(g,nx,nz)
dx=pi*g.D/nx; dz=g.L/nz;
theta=((1:nx)-.5)'*2*pi/nx;
zz=((1:nz)-.5)*dz-g.L/2;
[th,z]=ndgrid(theta,zz); ids=reshape(1:nx*nz,nx,nz);
S=zeros(nx*nz,g.rows*g.holes);
j=0;
for za=[-g.L/4,g.L/4]
    for i=0:g.holes-1
        j=j+1; a=i*2*pi/g.holes;
        ax=(g.D/2)*atan2(sin(th-a),cos(th-a));
        dsq=ax.^2+(z-za).^2;
        ww=exp(-dsq/(2*g.spread^2)).*(dsq<(3.5*g.spread)^2);
        S(:,j)=ww(:)/sum(ww(:));
    end
end
gg=struct('dx',dx,'dz',dz,'theta',th(:),'cos',cos(th(:)),'sin',sin(th(:)), ...
    'N',nx*nz,'S',sparse(S),'ix',ids(:),'jx',reshape(circshift(ids,-1,1),[],1), ...
    'iz',reshape(ids(:,1:end-1),[],1),'jz',reshape(ids(:,2:end),[],1), ...
    'bnd',[ids(:,1);ids(:,end)]);
assert(max(abs(full(sum(gg.S,1))-1))<1e-10,'Non-conservative source weights');
end

function m=orifice(g,d,r)
% Isentropic ideal gas orifice. Choking at the critical pressure ratio.
gm=g.gamma;
crit=(2/(gm+1))^(gm/(gm-1));
rr=max(crit,min(1,r));
m=g.Cd*pi*d^2/4*g.ps/sqrt(g.Rg*g.T).* ...
    sqrt(max(0,2*gm/(gm-1)*(rr.^(2/gm)-rr.^((gm+1)/gm))));
end

function r=radial(g,gg,e)
% Cell-centred finite volumes. p is scaled by supply absolute pressure.
% Positive mass flux leaves the cell. The Couette term is centred in theta.
h=g.c-e(1)*gg.cos-e(2)*gg.sin;
assert(min(h)>0,'Gas film closes');
U=pi*g.D*g.rpm/60; T=g.Rg*g.T;
ii=[gg.ix;gg.iz]; jj=[gg.jx;gg.jz];
hh3=2*h(ii).^3.*h(jj).^3./(h(ii).^3+h(jj).^3);
G=hh3.*[repmat(gg.dz/gg.dx,gg.N,1);repmat(gg.dx/gg.dz,numel(gg.iz),1)]*g.ps^2/(24*g.mu*T);
bg=h(gg.bnd).^3*(2*gg.dx/gg.dz)*g.ps^2/(24*g.mu*T);
L=sparse([ii;ii;jj;jj;gg.bnd],[ii;jj;ii;jj;gg.bnd],[G;-G;-G;G;bg],gg.N,gg.N);
a=(h(gg.ix)+h(gg.jx))/2*(U*gg.dz*g.ps/(4*T));
B=sparse([gg.ix;gg.ix;gg.jx;gg.jx],[gg.ix;gg.jx;gg.ix;gg.jx],[a;a;-a;-a],gg.N,gg.N);
alpha=g.pa/g.ps;
bc=accumarray(gg.bnd,bg*alpha^2,[gg.N,1]);
SS=gg.S; nsrc=size(SS,2); p=ones(gg.N,1)*.49;
for j=1:35
    pp=SS.'*p;
    mo=orifice(g,g.d,pp);
    res=L*(p.^2)+B*p-bc-SS*mo;
    er=norm(res,inf);
    if er<2e-14, break; end
    dd=1e-5;
    dm=(orifice(g,g.d,pp+dd)-orifice(g,g.d,pp-dd))/(2*dd);
    J=L*spdiags(2*p,0,gg.N,gg.N)+B-SS*spdiags(dm,0,nsrc,nsrc)*SS.';
    dp=-(J\res); step=1;
    for i=1:16
        pn=p+step*dp;
        if min(pn)>.5*alpha && max(pn)<1.05 && ...
                norm(L*(pn.^2)+B*pn-bc-SS*orifice(g,g.d,SS.'*pn),inf)<er
            break;
        end
        step=step/2;
    end
    p=pn;
end
assert(er<2e-14,'Pressure solve did not converge');
pph=g.ps*p;
F=-gg.dx*gg.dz*[gg.cos.';gg.sin.']*(pph-g.pa);
flow=sum(mo);
exitFlow=sum(bg.*(p(gg.bnd).^2-alpha^2));
r=struct('F',F,'Q',flow/(g.pa/T)*60000,'err',abs(flow-exitFlow)/flow, ...
    'pmin',min(pph),'pmax',max(pph),'hmin',(g.c-norm(e))*1e6);
end

function J=radialForceMatrix(g,gg,e,d)
J=zeros(2);
for i=1:2
    de=zeros(2,1); de(i)=d;
    a=radial(g,gg,e+de); b=radial(g,gg,e-de);
    J(:,i)=(a.F-b.F)/(2*d);
end
end

function q=radialEquilibrium(g,gg,load,e)
for j=1:14
    r=radial(g,gg,e); target=r.F+[load;0];
    if norm(target)<1e-5, break; end
    J=radialForceMatrix(g,gg,e,.07e-6);
    delta=-(J\target);
    sc=1;
    while norm(e+sc*delta)>.88*g.c
        sc=sc/2;
        assert(sc>1e-4,'Equilibrium cannot be found before film collapse');
    end
    e=e+sc*delta;
end
assert(norm(target)<1e-5,'Static force equilibrium did not converge');
q=struct('offset',(e*1e6).','e',norm(e)*1e6,'reaction',r.F.', ...
    'hmin',r.hmin,'Q',r.Q,'massError',r.err,'pmin',r.pmin,'pmax',r.pmax);
end

function a=axial(g,h)
% Axisymmetric double-land annular bearing. Groove pressure is uniform.
li=log(g.rg/g.ri); lo=log(g.ro/g.rg);
C=pi*h^3/(12*g.mu*g.Rg*g.T)*(1/li+1/lo);
r=fzero(@(x) orifice(g,g.da,x)-C*((g.ps*x)^2-g.pa^2),[g.pa/g.ps,1]);
pg=g.ps*r; v=pg^2-g.pa^2;
Fi=integral(@(x) 2*pi*x.*(sqrt(g.pa^2+v*log(x/g.ri)/li)-g.pa),g.ri,g.rg);
Fo=integral(@(x) 2*pi*x.*(sqrt(g.pa^2+v*log(g.ro./x)/lo)-g.pa),g.rg,g.ro);
q=C*v/(g.pa/(g.Rg*g.T))*60000;
a=struct('pg',pg,'F',Fi+Fo,'Q',q);
end
